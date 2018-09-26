using Abp.Dependency;
using FortuneLab.Models;
using FortuneLab.Runtime.Session;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Caches;
using FortuneLab.WebApiClient.Query;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.WebSockets;
using FortuneLab.Authorizations;
using StackExchange.Redis;

namespace FortuneLab.ECService.Securities
{
    /// <summary>
    /// 服务端授权服务
    /// </summary>
    public class AuthorizationService
    {
        private const string InstanceCacheName = "AuthorizationServiceRequestInstance";

        public static AuthorizationService Instance
        {
            get
            {
                if (System.Web.HttpContext.Current.Items[InstanceCacheName] == null)
                {
                    System.Web.HttpContext.Current.Items.Add(InstanceCacheName, new AuthorizationService());
                }

                return System.Web.HttpContext.Current.Items[InstanceCacheName] as AuthorizationService;
            }
        }

        private AuthorizationService()
        {

        }

        /// <summary>
        /// 清楚授权缓存
        /// </summary>
        /// <param name="userId">为空则只清楚当前用户的缓存</param>
        public void ClearAuthorizationCache(int? userId = null)
        {
            userId = userId ?? CurrentUserHelper.GetCurrentUserId();
            RedisFunctionAuthorizationServivce.RemoveFunctionListCache(userId.Value);
            RedisRoleAuthorizationService.RemoveRoleListCache(userId.Value);
        }

        public bool RoleAuthorize(string roleName, int? userId = null)
        {
            return RedisRoleAuthorizationService.RoleAuthorize(roleName, userId ?? CurrentUserHelper.GetCurrentUserId());
        }

        public bool FunctionAuthorize(string authKey, int? userId = null)
        {
            return RedisFunctionAuthorizationServivce.FunctionAuthorize(authKey, userId ?? CurrentUserHelper.GetCurrentUserId());
        }
    }

    internal static class CurrentUserHelper
    {
        public static int GetCurrentUserId()
        {
            var session = IocManager.Instance.Resolve<IFortuneLabSession>();
            return session.Identity.UserId;
        }
    }
}
