using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Redis;


namespace NBK.ECService.WMSReport.Utility.Redis
{
   public class RedisWMS
    {
        private static RedisClient RedisClientFactory = null;

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <param name="key">在RedisSourceKey类内有维护具体缓存名称</param>
        /// <returns></returns>
        public static TResult CacheResult<TResult>(Func<TResult> action, string key)
        {
            var tResult = GetRedisList<TResult>(key);
            if (tResult == null)
            {
                tResult = action();
                SetRedis(tResult, key);
            }
            return tResult;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <param name="key">在RedisSourceKey类内有维护具体缓存名称</param>
        /// <returns></returns>
        public static TResult CacheResult<TResult>(Func<TResult> action, string key, TimeSpan expiresIn)
        {
            var tResult = GetRedisList<TResult>(key);
            if (tResult == null)
            {
                tResult = action();
                SetRedis(tResult, key, expiresIn);
            }
            return tResult;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <param name="key">在RedisSourceKey类内有维护具体缓存名称</param>
        /// <returns></returns>
        public static TResult CacheResult<TResult>(Func<TResult> action, string key, DateTime expiresIn)
        {
            var tResult = GetRedisList<TResult>(key);
            if (tResult == null)
            {
                tResult = action();
                SetRedis(tResult, key, expiresIn);
            }
            return tResult;
        }

        /// <summary>
        /// 获取集合List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elementKey"></param>
        /// <returns></returns>
        public static TResult GetRedisList<TResult>(string key)
        {
            TResult result = default(TResult);
            try
            {
                using (
                    RedisClientFactory =
                        new RedisClient(PublicConst.RedisWMSHostName))
                {
                    result = RedisClientFactory.Get<TResult>(key);
                }
            }
            catch (Exception ex)
            {
                //异常日志
            }
            return result;
        }

        /// <summary>
        /// 清除T缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void CleanRedis<T>(string key)
        {
            try
            {

                using (
                    RedisClientFactory =
                        new RedisClient(PublicConst.RedisWMSHostName))
                {
                    RedisClientFactory.Del(key);
                }
            }
            catch (Exception ex)
            {
                //异常日志
            }
        }

        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public static void SetRedis<TResult>(TResult t, string key)
        {
            try
            {
                using (
                    RedisClientFactory = new RedisClient(PublicConst.RedisWMSHostName))
                {
                    RedisClientFactory.Set(key, t);
                }
            }
            catch (Exception ex)
            {
                //异常日志
            }
        }

        public static void SetRedis<TResult>(TResult t, string key, TimeSpan expiresIn)
        {
            try
            {
                using (
                    RedisClientFactory = new RedisClient(PublicConst.RedisWMSHostName))
                {
                    RedisClientFactory.Set(key, t, expiresIn);
                }
            }
            catch (Exception ex)
            {
                //异常日志
            }
        }


        public static void SetRedis<TResult>(TResult t, string key, DateTime expiresIn)
        {
            try
            {
                using (
                    RedisClientFactory = new RedisClient(PublicConst.RedisWMSHostName))
                {
                    RedisClientFactory.Set(key, t, expiresIn);
                }
            }
            catch (Exception ex)
            {
                //异常日志
            }
        }

        /// <summary>
        /// 对象序列号
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        private static byte[] Serialize<T>(T t)
        {
            MemoryStream mStream = new MemoryStream();
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(mStream, t);
            return mStream.GetBuffer();
        }

        /// <summary>
        /// 对象反序列话
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="b"></param>
        /// <returns></returns>
        private static T Deserialize<T>(byte[] b)
        {
            BinaryFormatter bFormatter = new BinaryFormatter();
            return (T)bFormatter.Deserialize(new MemoryStream(b));
        }
    }
}
