using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace FortuneLab.Caches.Redis
{
    public class RedisStore
    {
        private static readonly int FrameworkDatabaseIndex;
        private static readonly int BusinessDatabaseIndex;
        private static ConnectionMultiplexer _connection;
        private static readonly object ConnectionLocker = new object();

        static RedisStore()
        {
            var frameworkDatabase = ConfigurationManager.AppSettings["RedisServer:FrameworkDatabase"];
            if (string.IsNullOrWhiteSpace(frameworkDatabase))
            {
                throw new Exception("AppSetting中没有找到RedisServer:FrameworkDatabase的配置");
            }

            int frameworkDatabaseIndex = 0;
            if (!int.TryParse(frameworkDatabase, out frameworkDatabaseIndex))
            {
                throw new Exception("AppSetting中RedisServer:FrameworkDatabase的Value必须为数字");
            }

            FrameworkDatabaseIndex = frameworkDatabaseIndex;

            var businesskDatabase = ConfigurationManager.AppSettings["RedisServer:BusinessDatabase"];
            if (string.IsNullOrWhiteSpace(businesskDatabase))
            {
                throw new Exception("AppSetting中没有找到RedisServer:BusinessDatabaseIndex");
            }

            int businessDatabaseIndex = 0;
            if (!int.TryParse(businesskDatabase, out businessDatabaseIndex))
            {
                throw new Exception("AppSetting中RedisServer:BusinessDatabaseIndex");
            }

            BusinessDatabaseIndex = businessDatabaseIndex;
        }

        public static ConnectionMultiplexer Connection
        {
            get
            {

                if (_connection != null && _connection.IsConnected)
                    return _connection;

                lock (ConnectionLocker)
                {
                    if (_connection != null && _connection.IsConnected)
                        return _connection;

                    var redisEndpoint = ConfigurationManager.AppSettings["RedisServer:Host"];
                    if (string.IsNullOrWhiteSpace(redisEndpoint))
                    {
                        throw new Exception("AppSetting中没有找到RedisServer:Host的配置");
                    }

                    var configurationOptions = new ConfigurationOptions
                    {
                        EndPoints = { redisEndpoint },
                        //AbortOnConnectFail = false
                    };

                    try
                    {
                        _connection = ConnectionMultiplexer.Connect(configurationOptions);
                        return _connection;
                    }
                    catch (RedisConnectionException ex)
                    {
                        throw new Exception($"Redis服务器连接失败, 连接地址:{redisEndpoint}, 错误信息:{ex.Message}", ex);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Redis服务器连接失败, 连接地址:{redisEndpoint}, 错误信息:{ex.Message}", ex);
                    }
                }
            }
        }

        /// <summary>
        /// 获取一个新的Framework Redis Database 对象
        /// </summary>
        public static IDatabase FrameworkRedisCache => GetRedisCache(FrameworkDatabaseIndex);
        public static IDatabase BusinessRedisCache => GetRedisCache(FrameworkDatabaseIndex);

        /// <summary>
        /// 获取一个新的Redis Database 对象
        /// </summary>
        /// <param name="databaseIndex"></param>
        /// <returns></returns>
        public static IDatabase GetRedisCache(int databaseIndex)
        {
            return Connection.GetDatabase(FrameworkDatabaseIndex);
        }
    }
}
