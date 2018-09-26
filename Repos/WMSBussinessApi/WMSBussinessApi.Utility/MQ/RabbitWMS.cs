using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMSBussinessApi.Utility.Enum;
using WMSBussinessApi.Utility.Helper;

namespace WMSBussinessApi.Utility.MQ
{
    public class RabbitWMS
    {
        private static ConnectionFactory factory = null;
        static RabbitWMS()
        {
            factory = new ConnectionFactory()
            {
                HostName = PublicConst.RabbitWMSHostName,
                UserName = PublicConst.RabbitWMSUserName,
                Password = PublicConst.RabbitWMSPassword
            };
        }

        /// <summary>
        /// 异步写入消息队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rabbitMQType"></param>
        /// <param name="entity"></param>
        public static void SetRabbitMQAsync<T>(RabbitMQType rabbitMQType, T entity)
        {
            new Task(() =>
            {
                try
                {
                    using (var connection = factory.CreateConnection())
                    {
                        using (var channel = connection.CreateModel())
                        {
                            bool durable = true;
                            channel.QueueDeclare(rabbitMQType.ToDescription(), durable, false, false, null);
                            var properties = channel.CreateBasicProperties();
                            properties.Persistent = true;

                            //此处防止时间格式序列化跨时区
                            Newtonsoft.Json.Converters.IsoDateTimeConverter timeFormat = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

                            channel.BasicPublish(PublicConst.Exchange, rabbitMQType.ToDescription(), properties, ConvertBytes.SerializeBinary(JsonConvert.SerializeObject(entity, Formatting.Indented, timeFormat)));

                        }
                    }
                }
                catch (Exception ex)
                {
                    //推送异常先忽略
                }
            }).Start();
        }

        /// <summary>
        /// 同步推送MQ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rabbitMQType"></param>
        /// <param name="entity"></param>
        public static void SetRabbitMQSync<T>(RabbitMQType rabbitMQType, T entity)
        {
            try
            {
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        bool durable = true;
                        channel.QueueDeclare(rabbitMQType.ToDescription(), durable, false, false, null);
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        //此处防止时间格式序列化跨时区
                        Newtonsoft.Json.Converters.IsoDateTimeConverter timeFormat = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                        timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

                        channel.BasicPublish(PublicConst.Exchange, rabbitMQType.ToDescription(), properties, ConvertBytes.SerializeBinary(JsonConvert.SerializeObject(entity, Formatting.Indented, timeFormat)));
                    }
                }
            }
            catch (Exception ex)
            {
                //推送异常先忽略
            }
        }

        /// <summary>
        /// 读取消息队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetRabbitMQ<T>(RabbitMQType rabbitMQType)
        {
            T result = default(T);
            try
            {
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        bool durable = true;
                        channel.QueueDeclare(rabbitMQType.ToDescription(), durable, false, false, null);
                        channel.BasicQos(0, 1, false);

                        var consumer = new QueueingBasicConsumer(channel);
                        channel.BasicConsume(rabbitMQType.ToDescription(), false, consumer);

                        while (true)
                        {
                            var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                            result = ConvertBytes.DeserializeBinary<T>(ea.Body);

                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}
