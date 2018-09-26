using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NLog;
using RabbitMQ.Client.Exceptions;

namespace FortuneLab.MessageQueue.RabbitMq
{
    public class ConnectionManager
    {

        private static readonly object ConnectionLocker = new object();

        public static ConnectionManager Instance = new ConnectionManager();

        private IModel _currentModel;
        private static readonly object ChannelLocker = new object();

        private string ConfigurationName { get; set; }

        private readonly Logger _logger = LogManager.GetLogger(typeof(ConnectionManager).FullName);

        public ConnectionManager()
            : this("RabbitServer")
        {

        }

        public ConnectionManager(string configurationName)
        {
            this.ConfigurationName = configurationName;
        }

        public IConnection GetNewConnection()
        {
            var connString = GetConnString();
            var connStringBuilder = ConnectionStringBuilder.BuildConnectionString(connString);
            var factory = new ConnectionFactory()
            {
                HostName = connStringBuilder.HostName,
                UserName = connStringBuilder.UserName,
                Password = connStringBuilder.Password,
                VirtualHost = connStringBuilder.VirtualHost
            };

            IConnection connection = null;
            var quickRetryTimes = 0;

            do
            {
                try
                {
                    connection = factory.CreateConnection();
                }
                catch (BrokerUnreachableException ex)
                {
                    _logger.Fatal(ex, "Connecting to RabbitMQ Server failure, Server Address: {0}", connString);
                    if (++quickRetryTimes <= 4)
                    {
                        //前三次每15秒通知一次，后续每两分钟通知一次, 避免太多的邮件通知
                        Console.WriteLine("Connecting to RabbitMQ Server failure, Wait 15 second and continue try");
                        System.Threading.Thread.Sleep(15 * 1000);
                    }
                    else
                    {
                        Console.WriteLine("Connecting to RabbitMQ Server failure, Wait 120 second and continue try");
                        System.Threading.Thread.Sleep(2 * 60 * 1000);
                    }
                }
                finally
                {

                }

            } while (connection == null);

            return connection;
        }

        private string GetConnString()
        {
            var appConnString = ConfigurationManager.AppSettings[this.ConfigurationName];
            if (string.IsNullOrWhiteSpace(appConnString))
                throw new Exception($"请配置名称{appConnString}的RabbitMQ连接地址");

            return appConnString;
        }

        /// <summary>
        /// 获取当前程序中Channel的单例实例
        /// </summary>
        /// <returns></returns>
        public IModel GetCurrentModel()
        {
            if (_currentModel != null && !_currentModel.IsClosed) return _currentModel;
            lock (ChannelLocker)
            {
                if (_currentModel != null && !_currentModel.IsClosed) return _currentModel;
                var connection = GetNewConnection();

                _currentModel = connection.CreateModel();
            }
            return _currentModel;
        }

        /// <summary>
        /// 创建一个新的Channel
        /// </summary>
        /// <returns></returns>
        public IModel GetNewChannel()
        {
            var connection = GetNewConnection();
            return connection.CreateModel();
        }
    }
}
