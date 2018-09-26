using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using FortuneLab.MessageQueue.MessageModels;
using FortuneLab.Caches.Redis;

namespace FortuneLab.MessageQueue.RabbitMq
{
    public abstract class MessageConsumer<TEventData> : IMessageConsumer
    {
        public string SessionName { get; private set; }
        private ILogger _logger = LogManager.GetLogger(typeof(MessageConsumer<>).FullName);
        private IModel _channel = null;
        protected CancellationTokenSource CancellationToken { get; private set; }

        readonly object _channelLocker = new object();
        protected IModel GetChannelInstance(ConnectionManager connectionManager = null)
        {
            if (_channel != null && !_channel.IsClosed) return _channel;

            lock (_channelLocker)
            {
                var isClosed = _channel != null && _channel.IsClosed;//记录一个是否是异常关闭的状态，如果是，后面会针对恢复做一些处理
                if (_channel != null && !_channel.IsClosed) return _channel;

                if (_channel != null)
                {
                    _channel.Dispose(); //释放原有资源 
                    _channel.Close();
                    _channel = null;
                }

                if (connectionManager == null)
                    connectionManager = ConnectionManager.Instance;

                _channel = connectionManager.GetNewChannel();
                if (isClosed)
                {
                    ConsoleLog("{0} _channel connection is recovery", SessionName);
                }
            }
            return _channel;
        }

        public string ExchangeName { get; protected set; }
        public string QueueName { get; protected set; }
        public bool IsStoped { get; private set; }
        protected virtual int RetryTimes { get; private set; }
        protected virtual bool RetryEnable { get; private set; }

        protected virtual bool IsMoveToErrorQueueWhenExceptionOccured { get; set; }

        protected virtual ConnectionManager ConnectionManager { get; set; }
        public MessageConsumer(CancellationTokenSource cancellationToken, int retryTimes = 3, bool retryEnable = true, bool isMoveToErrorQueueWhenExceptionOccured = true)
        {
            this.CancellationToken = cancellationToken;
            this.RetryTimes = retryTimes;
            this.RetryEnable = retryEnable;
            this.IsMoveToErrorQueueWhenExceptionOccured = IsMoveToErrorQueueWhenExceptionOccured;
        }

        protected abstract void InitChannel();

        public void StartConsume(string sessionName)
        {
            this.SessionName = sessionName;
            InitChannel();

            if (ConnectionManager == null)
                ConnectionManager = ConnectionManager.Instance;

            var channel = GetChannelInstance(ConnectionManager);
            channel.ModelShutdown += ChannelShutdownHandler;

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += ConsumerReceiveHandler;
            channel.BasicConsume(queue: QueueName, noAck: false, consumer: consumer);
        }

        protected virtual void ChannelShutdownHandler(object sender, ShutdownEventArgs e)
        {
            _channel.Dispose();
            _channel.Close();

            ConsoleLog(SessionName + " Channel_ModelShutdown");
            if (!CancellationToken.IsCancellationRequested)
            {
                Task.Run(() =>
                {
                    InitChannel();
                    StartConsume(SessionName);
                });
            }
        }

        public virtual void StopConsume()
        {
            if (_channel != null)
            {
                _channel.Dispose();
                _channel = null;
            }
            IsStoped = true;
        }

        protected virtual void ConsumerReceiveHandler(object sender, BasicDeliverEventArgs ea)
        {
            StopwatchAction(() =>
            {
                if (CancellationToken.IsCancellationRequested)
                {
                    StopConsume();//释放本地对象
                    return;
                }

                ConsumeHandleResult consumeHandleResult = ConsumeHandleResult.Act;
                Exception consumeException = null;
                TEventData eventData = default(TEventData);
                try
                {
                    var bodyMessage = Encoding.UTF8.GetString(ea.Body);
                    eventData = JsonConvert.DeserializeObject<TEventData>(bodyMessage);
                    Consumer(ea, eventData, bodyMessage);
                    consumeHandleResult = ConsumeHandleResult.Act;
                }
                catch (JsonException ex)
                {
                    consumeException = ex;
                    _logger.Error(ex, $"消息体反序列化错误:错误信息{ex.FullMessage()}");
                    consumeHandleResult = ConsumeHandleResult.ActAndMoveToErrorQueue;
                }
                catch (Exception ex)
                {
                    consumeException = ex;
                    if (!IsMoveToErrorQueueWhenExceptionOccured)
                    {
                        //如果屏蔽了转发, 直接忽略消费失败
                        consumeHandleResult = ConsumeHandleResult.Act;
                    }

                    _logger.Error(ex, $"消息消费失败:错误信息{ex.FullMessage()}");
                    consumeHandleResult = ConsumeHandleResult.ActAndMoveToErrorQueue;//出现异常, 默认是承认消息, 并转移到错误消息队列

                    if (RetryEnable && eventData is IEventMessageData)
                    {
                        var retrySupportEventData = eventData as IEventMessageData;
                        if (retrySupportEventData.SupportRetry && retrySupportEventData.MessageId != Guid.Empty)//如果MessageId为空, 直接进入错误队列
                        {
                            var redis = RedisStore.FrameworkRedisCache;

                            var cacheKey = $"Framework:Consumer:FailureConsumer:RetryCount:{retrySupportEventData.MessageId}";
                            var retryTimesValue = redis.StringGet(cacheKey);
                            redis.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(5));

                            int retryTimes = 0;
                            retryTimesValue.TryParse(out retryTimes);

                            if (retryTimes < this.RetryTimes)
                            {
                                redis.StringIncrement(cacheKey);
                                consumeHandleResult = ConsumeHandleResult.Reject;
                            }
                        }
                    }
                }

                switch (consumeHandleResult)
                {
                    case ConsumeHandleResult.Act:
                        (sender as IBasicConsumer).Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        break;
                    case ConsumeHandleResult.ActAndMoveToErrorQueue:
                        (sender as IBasicConsumer).Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        MoveMessageToErrorQueue(ea, Encoding.UTF8.GetString(ea.Body), consumeException);
                        break;
                    case ConsumeHandleResult.Reject:
                        (sender as IBasicConsumer).Model.BasicReject(ea.DeliveryTag, true);//拒绝消息, 重新插入队列
                        break;
                    default:
                        break;
                }

            }, SessionName + " consumer message used:{0} ms", sw =>
            {
                //var redisClient = FortuneLab.Caches.Redis.RedisStore.FrameworkRedisCache;
                //var key = $"Consumers:{this.GetType().Name}:{ea.ConsumerTag}";

                //var consumeTimeKey = $"{key}:LastConsumeTime";
                //redisClient.StringSetAsync(consumeTimeKey, DateTime.Now.ToString());
                //redisClient.KeyExpireAsync(consumeTimeKey, TimeSpan.FromDays(1));

                //var durationKey = $"{key}:ConsumeDuration";
                //redisClient.ListLeftPushAsync(durationKey, sw.ElapsedMilliseconds);
                //redisClient.ListTrimAsync(durationKey, 0, 98);
                //redisClient.KeyExpireAsync(durationKey, TimeSpan.FromDays(1));
            });
        }

        internal enum ConsumeHandleResult
        {
            Act,
            ActAndMoveToErrorQueue,
            Reject
        }


        private void MoveMessageToErrorQueue(BasicDeliverEventArgs ea, string bodyMessage, Exception ex)
        {
            var errorQueue = "FX_EX_ConsumeError";
            ConsumeErrorMessage errorMessage = new ConsumeErrorMessage()
            {
                Exchange = this.ExchangeName,
                Queue = this.QueueName,
                RoutingKey = ea.RoutingKey,
                MessageContent = bodyMessage,
                ExceptionMessage = ex.FullMessage(),
                Exception = ex
            };
            RabbitMqChannelPool.PubMessage((channel) =>
            {
                channel.ExchangeDeclare(exchange: errorQueue, type: "topic", durable: true, autoDelete: false, arguments: null);
                channel.BasicPublish(errorQueue, ea.RoutingKey, basicProperties: null, body: MessageHelper.GetMessageBody(errorMessage));
            });
        }

        public abstract void Consumer(BasicDeliverEventArgs e, TEventData message, string bodyMessage);

        private void ConsoleLog(string format, params object[] parms)
        {
            Console.WriteLine(format, parms);
        }

        private void StopwatchAction(Action action, string format, Action<Stopwatch> callback = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            _logger.Trace(format, sw.ElapsedMilliseconds);
            if (callback != null)
            {
                try
                {
                    callback(sw);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 获取MQ绑定时的参数
        /// </summary>
        /// <returns></returns>
        public virtual IDictionary<string, object> GetConsumerParameters()
        {
            var pams = new Dictionary<string, object>();//{ { "name":"ApplicationInstantLoading" } };
            pams.Add("ConsumerSessionName", SessionName);
            pams.Add("MachineName", Dns.GetHostName());
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] addr = ipEntry.AddressList;
            for (int i = 0; i < addr.Length; i++)
            {
                pams.Add("ip-" + i, addr[i].ToString());
            }
            return pams;
        }
    }

    public class ConsumeErrorMessage
    {
        public string Exchange { get; set; }
        public string Queue { get; set; }
        public string RoutingKey { get; set; }
        public string MessageContent { get; set; }
        public string ExceptionMessage { get; set; }
        public Exception Exception { get; set; }
    }
}
