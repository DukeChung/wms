using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSBussinessApi.Utility.MQ
{
    public class MQConsumer
    {
        private string _hostName;
        private int _hostPort;
        private string _queueName;
        private string _userName;
        private string _password;
        private string _virtualHost;
        public MQConsumer(string hostName, int hostPort, string userName, string password, string queueName, string virtualHost = "ecc_host")
        {
            _hostName = hostName;       //"127.0.0.1"
            _hostPort = hostPort;       //5672
            _queueName = queueName;
            _userName = userName;
            _password = password;
            _virtualHost = virtualHost;
        }

        public void Run(Action<string> callback)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = _hostName;
            //factory.Port = _hostPort;
            factory.UserName = _userName;
            factory.Password = _password;
            factory.VirtualHost = _virtualHost;

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.BasicQos(0, 1, false);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        callback(message);
                        channel.BasicAck(deliveryTag:ea.DeliveryTag, multiple:false);   //手动确认
                    };
                    channel.BasicConsume(queue: _queueName, noAck: false, consumer: consumer);  //不自动确认
                    /*
                     * 如果要自动确认,则
                     * 1. 删掉 手动确认 代码
                     * 2. 将不自动确认改为 channel.BasicConsume(queue: _queueName, noAck: true, consumer: consumer);  
                     */
                }
            }
        }
    }
}
