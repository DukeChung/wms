using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMSBussinessApi.Utility.MQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "10.66.150.102";
            //factory.Port = _hostPort;
            factory.UserName = "ecc_admin";
            factory.Password = "setpay@123";
            factory.VirtualHost = "ecc_host";

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "jelax.dexchange_DefaultQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.BasicQos(0, 1, false);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine($"wo huoqu dao de xiaoxi:{message}");
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: "jelax.dexchange_DefaultQueue", noAck: false, consumer: consumer);

                }
            }

            Console.WriteLine("------------dengdaixiaoxizhong--------------");
            Console.ReadKey();
        }
    }
}
