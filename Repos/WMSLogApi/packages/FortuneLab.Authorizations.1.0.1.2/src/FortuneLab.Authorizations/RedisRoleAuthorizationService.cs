using System;
using System.Linq;
using FortuneLab.WebApiClient.Caches;
using StackExchange.Redis;
using FortuneLab.Caches.Redis;

namespace FortuneLab.Authorizations
{
    public static class RedisRoleAuthorizationService
    {
        const string UserRoleCacheKey = "Framework:Authorization:UserRoleNames:User:{0}";

        private static string GetCacheKey(int userId)
        {
            return string.Format(UserRoleCacheKey, userId);
        }

        #region SystemRole Authorization
        public static void RemoveRoleListCache(int userId)
        {
            var redisClient = RedisStore.FrameworkRedisCache;
            redisClient.KeyDelete(GetCacheKey(userId));
        }

        public static bool RoleAuthorize(string roleName, int userId)
        {
            var redisClient = RedisStore.FrameworkRedisCache;

            var storeKey = GetCacheKey(userId);
            var length = redisClient.SetLength(storeKey);
            if (length >= 1)
                return redisClient.SetContains(storeKey, roleName);

            var rsp = PermissionApiClient.GetSystemRoleNames(userId);
            if (rsp.Success && rsp.ResponseResult.Count > 0)
            {
                redisClient.SetAdd(storeKey, rsp.ResponseResult.Select(x => (RedisValue)x).ToArray());
                redisClient.KeyExpire(storeKey, TimeSpan.FromMinutes(10));
            }
            return redisClient.SetContains(storeKey, roleName);
        }
        #endregion
    }
}
