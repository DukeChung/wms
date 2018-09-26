using FortuneLab.Caches;
using System;
using System.Configuration;

namespace FortuneLab.WebApiClient.Caches
{
    /// <summary>
    /// 全局公共对象Cache
    /// </summary>
    public class CommonObjectCache
    {
        private static CommonObjectCache Instance { get; } = new CommonObjectCache();
        private readonly ICommonOjbectCacheProvider _cacheProvider = null;

        private CommonObjectCache()
        {
            var cacheProviderTypeString = ConfigurationManager.AppSettings["ObjectCacheProvider"] ?? "FortuneLab.WebApiClient.Caches.MemoryObjectCacheProvider";
            var cacheProviderType = Type.GetType(cacheProviderTypeString);
            if (cacheProviderType == null)
            {
                throw new Exception($"Can't find type {cacheProviderTypeString}");
            }
            _cacheProvider = Activator.CreateInstance(cacheProviderType) as ICommonOjbectCacheProvider;
        }

        public T GetFromCache<T>(string cacheKey)
            where T : class
        {
            if (!_cacheProvider.KeyExists(cacheKey))
            {
                return null;
            }

            return _cacheProvider.Get<T>(cacheKey);
        }

        /// <summary>
        /// Milliseconds
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="data"></param>
        /// <param name="expiry"></param>
        public void SetToCache<T>(string cacheKey, T data, int expiry = 1000 * 60)
        {
            if (data == null)
                return;

            _cacheProvider.Set(cacheKey, data, TimeSpan.FromMilliseconds(expiry));
        }

        public void RemoveCache(string dataKey)
        {
            _cacheProvider.KeyDelete(dataKey);
        }

        /// <summary>
        /// 缓存某一个对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public static TResult CacheResult<TResult>(Func<TResult> action, string cacheKey, Action<bool> CacheResultAction = null, int expiry = 1000 * 60 * 5) where TResult : class
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
                throw new Exception("DataKey不能为空");

            var cachedData = Instance.GetFromCache<TResult>(cacheKey);
            if (cachedData != null)
            {
                CacheResultAction?.Invoke(true);
                return cachedData;
            }

            var result = action();
            Instance.SetToCache(cacheKey, result, expiry);
            CacheResultAction?.Invoke(false);
            return result;
        }

        public static TResult Get<TResult>(string cacheKey) where TResult : class
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
                throw new Exception("DataKey不能为空");

            var cachedData = Instance.GetFromCache<TResult>(cacheKey);
            return cachedData;
        }

        /// <summary>
        /// 移除某个对象
        /// </summary>
        /// <param name="cacheKey"></param>
        public static void RemoveKey(string cacheKey)
        {
            Instance.RemoveCache(cacheKey);
        }
    }
}
