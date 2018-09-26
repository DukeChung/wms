using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSBussinessApi.Utility.MQ
{
    public class MQProducer
    {
        private string _hostName;
        private int _hostPort;
        private string _userName;
        private string _password;
        private string _virtualHost;
        public MQProducer(string hostName, int hostPort, string userName, string password,string virtualHost="ecc_host")
        {
            _hostName = hostName;   //"127.0.0.1"
            _hostPort = hostPort;   //5672
            _userName = userName;
            _password = password;
            _virtualHost = virtualHost;
        }

        /// <summary>
        /// 将消息发送到direct exchange交换机上
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exchangeName"></param>
        public void SendMsgToDExchange(string msg, string exchangeName, string msgKey = "")
        {
            SendMsgToExchange(msg, exchangeName, "direct", msgKey);
        }


        public void SendMsgToTExchange(string msg, string exchangeName, string msgKey = "")
        {
            SendMsgToExchange(msg, exchangeName, "topic", msgKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exchangeName">不能有*、#</param>
        /// <param name="exchangeType"></param>
        /// <param name="msgKey"></param>
        private void SendMsgToExchange(string msg, string exchangeName,string exchangeType, string msgKey = "") 
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = _hostName;
            //factory.Port = _hostPort; //使用默认端口,所以不用写,写了反而会错
            factory.UserName = _userName;
            factory.Password = _password;
            factory.VirtualHost = _virtualHost;

            using (IConnection conn = factory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    //创建默认绑定
                    string queueName = $"{exchangeName}_DefaultQueue";
                    channel.ExchangeDeclare(exchange: exchangeName, type: exchangeType, durable: true, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: queueName, durable: true, autoDelete: false, exclusive: false, arguments: null);
                    channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: queueName);

                    //发送消息
                    byte[] buffer = Encoding.UTF8.GetBytes(msg);
                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2;    //2表示消息要固化
                    channel.BasicPublish(exchange: exchangeName, routingKey: string.IsNullOrEmpty(msgKey) ? queueName : msgKey, basicProperties: properties, body: buffer);
                }
            }
        }
    }
}
