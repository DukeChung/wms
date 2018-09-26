using FortuneLab.Caches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebApiClient.Caches
{
    public class MemoryObjectCacheProvider : ICommonOjbectCacheProvider
    {
        private static readonly ObjectCache ObjectCache = MemoryCache.Default;
        private static readonly ICacheDataSerializer CacheDataSerializer = new JsonSerializer();

        public bool KeyExists(string cacheKey)
        {
            return ObjectCache.Contains(cacheKey);
        }

        public T Get<T>(string cacheKey)
        {
            var cacheItem = ObjectCache.GetCacheItem(cacheKey);
            return CacheDataSerializer.Deserialize<T>((Byte[])cacheItem.Value);
        }

        public void Set<T>(string cacheKey, T data, TimeSpan expiry)
        {
            var policy = new CacheItemPolicy { SlidingExpiration = expiry };
            ObjectCache.Set(cacheKey, CacheDataSerializer.Serialize(data), policy);
        }

        public void KeyDelete(string dataKey)
        {
            ObjectCache.Remove(dataKey);
        }
    }
}
