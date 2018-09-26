using System;
using System.Linq;
using FortuneLab.WebApiClient.Caches;
using StackExchange.Redis;
using FortuneLab.Caches.Redis;

namespace FortuneLab.Authorizations
{
    public static class RedisFunctionAuthorizationServivce
    {
        const string UserFunctionCacheKey = "Framework:Authorization:UserFunctionNames:User:{0}";

        private static string GetCacheKey(int userId)
        {
            return string.Format(UserFunctionCacheKey, userId);
        }

        /// <summary>
        /// 依据AuthCenter SystemFunction AuthKey做检查
        /// </summary>
        /// <param name="authKey"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool FunctionAuthorize(string authKey, int userId)
        {
            var redisClient = RedisStore.FrameworkRedisCache;

            string storeKey = GetCacheKey(userId);
            var length = redisClient.SetLength(storeKey);
            if (length >= 1)
                return redisClient.SetContains(storeKey, authKey);

            var rsp = PermissionApiClient.GetSystemFunctionAuthKeies(userId);
            if (rsp.Success && rsp.ResponseResult.Count > 0)
            {
                redisClient.SetAdd(storeKey, rsp.ResponseResult.Select(x => (RedisValue)x).ToArray());
                redisClient.KeyExpire(storeKey, TimeSpan.FromMinutes(10));
            }
            return redisClient.SetContains(storeKey, authKey);
        }

        public static void RemoveFunctionListCache(int userId)
        {
            var redisClient = RedisStore.FrameworkRedisCache;
            redisClient.KeyDelete(GetCacheKey(userId));
        }
    }
}
