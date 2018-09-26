using FortuneLab.Caches;
using FortuneLab.Caches.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.Caches.Redis
{
    public class RedisObjectCacheProvider : ICommonOjbectCacheProvider
    {
        private readonly RetryPolicy<RedisCacheTransientErrorDetectionStrategy> _retryPolicy =
              new RetryPolicy<RedisCacheTransientErrorDetectionStrategy>(new FixedInterval(3, TimeSpan.FromSeconds(2)));

        private static readonly ICacheDataSerializer CacheDataSerializer = new JsonSerializer();

        public bool KeyExists(string cacheKey)
        {
            var cacheClient = RedisStore.BusinessRedisCache;
            return _retryPolicy.ExecuteAction(() => cacheClient.KeyExists(cacheKey));
        }

        public T Get<T>(string cacheKey)
        {
            var cacheClient = RedisStore.BusinessRedisCache;
            var cachedValue = _retryPolicy.ExecuteAction(() => cacheClient.StringGet(cacheKey));
            return cachedValue.IsNullOrEmpty ? default(T) : CacheDataSerializer.Deserialize<T>(cachedValue);
        }

        public void Set<T>(string cacheKey, T data, TimeSpan expiry)
        {
            var cacheClient = RedisStore.BusinessRedisCache;

            _retryPolicy.ExecuteAction(() =>
            {
                var sData = CacheDataSerializer.Serialize(data);
                return cacheClient.StringSet(cacheKey, sData, expiry: expiry);
            });
        }

        public void KeyDelete(string cacheKey)
        {
            var cacheClient = RedisStore.BusinessRedisCache;
            _retryPolicy.ExecuteAction(() => cacheClient.KeyDelete(cacheKey));
        }
    }

    public class RedisCacheTransientErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        /// <summary>
        /// Custom Redis Transient Error Detenction Strategy must have been implemented to satisfy Redis exceptions.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public bool IsTransient(Exception ex)
        {
            Console.WriteLine(ex.Message);
            if (ex == null) return false;

            if (ex is TimeoutException) return true;

            if (ex is RedisServerException) return true;

            if (ex is RedisException) return true;

            if (ex.InnerException != null)
            {
                return IsTransient(ex.InnerException);
            }

            return false;
        }
    }
}
