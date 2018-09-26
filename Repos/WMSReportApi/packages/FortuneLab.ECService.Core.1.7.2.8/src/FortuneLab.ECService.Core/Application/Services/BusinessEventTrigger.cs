using FortuneLab.MessageQueue.MessageModels;
using FortuneLab.MessageQueue.RabbitMq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FortuneLab.ECService.Application.Services
{
    public static class BusinessEventTrigger
    {
        /// <summary>
        /// 向MQ发送消息
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="eventData"></param>
        /// <param name="useThreadPool"></param>
        /// <exception cref="Exception"></exception>
        public static void Publish<T>(string businessMessageExchangeName, string routingKey, BusinessEventData<T> eventData, bool useThreadPool = true)
              where T : class
        {
            Action<object> action = (o) => RabbitMqChannelPool.PubMessage(channel =>
            {
                var result = false;
                var retryTimes = 0;
                do
                {
                    try
                    {
                        if (eventData.MessageId == Guid.Empty)
                        {
                            eventData.MessageId = Guid.NewGuid();
                        }
                        channel.ExchangeDeclare(exchange: businessMessageExchangeName, type: "topic", durable: true, autoDelete: false, arguments: null);
                        channel.BasicPublish(businessMessageExchangeName, routingKey, basicProperties: null, body: MessageHelper.GetMessageBody(eventData));
                        result = true;
                    }
                    catch (RabbitMQ.Client.Exceptions.OperationInterruptedException ex)
                    {
                        //如果是RabbitMQ异常, 每隔200ms重试一次, 总重试三次, 如果依旧失败, 抛出异常
                        result = false;
                        retryTimes += 1;
                        if (retryTimes >= 3)
                            throw new Exception($"到MQ的业务消息发送失败, RoutingKey:{routingKey}, 详细错误:{ex.Message}");
                        Thread.Sleep(200);
                    }

                } while (!result);
            });
            if (useThreadPool)
            {
                ThreadPool.QueueUserWorkItem(state => action(state));
            }
            else
            {
                action(null);
            }
        }
    }
}
