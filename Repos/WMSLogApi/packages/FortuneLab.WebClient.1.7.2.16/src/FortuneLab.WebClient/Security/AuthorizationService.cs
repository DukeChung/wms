using FortuneLab.WebApiClient.Caches;
using FortuneLab.WebApiClient.Query;
using FortuneLab.WebClient.Models;
using FortuneLab.WebClient.Security.AuthClient;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using FortuneLab.Authorizations;
using FortuneLab.WebClient.Mvc;

namespace FortuneLab.WebClient.Security
{
    /// <summary>
    /// Portal端授权服务
    /// </summary>
    public class AuthorizationService
    {
        //private Dictionary<string, string> _claims = null;
        //public Dictionary<string, string> Claims
        //{
        //    get
        //    {
        //        var user = System.Web.HttpContext.Current.User;
        //        if (_claims == null && user.Identity is ClaimsIdentity)
        //        {
        //            _claims = ((ClaimsIdentity)user.Identity).Claims.ToDictionary(x => x.Type, x => x.Value);
        //            Debug.Assert(_claims != null, "claims != null");
        //        }
        //        return _claims;
        //    }
        //}

        const string AuthorizationServiceCacheKey = "AuthorizationServiceCacheKey";
        const string UserFunctionCacheKey = "userFunctionAuthKeies-{0}";
        const string UserRoleCacheKey = "userRoleNames-{0}";

        public static AuthorizationService Instance
        {
            get
            {
                if (System.Web.HttpContext.Current.Items[AuthorizationService.AuthorizationServiceCacheKey] == null)
                {
                    System.Web.HttpContext.Current.Items.Add(AuthorizationService.AuthorizationServiceCacheKey, new AuthorizationService());
                }
                return System.Web.HttpContext.Current.Items[AuthorizationService.AuthorizationServiceCacheKey] as AuthorizationService;
            }
        }

        private AuthorizationService()
        {

        }

        #region SystemFunction Manager
        //private List<string> GetSystemFunctionList(string sessionKey)
        //{
        //    var permissionList = CommonObjectCache.CacheResult(() =>
        //    {
        //        var rsp = PermissionApiClient.GetSystemFunctionAuthKeies(new SessionQuery() { SessionKey = sessionKey }, isFilterByApplication: false);
        //        return rsp.Success ? rsp.ResponseResult : null;
        //    }, string.Format(AuthorizationService.UserFunctionCacheKey, sessionKey));
        //    return permissionList;
        //}

        private void RemoveFunctionListCache(int userId)
        {
            RedisFunctionAuthorizationServivce.RemoveFunctionListCache(userId);
            //CommonObjectCache.RemoveKey(string.Format(AuthorizationService.UserFunctionCacheKey, sessionKey));
        }
        #endregion

        #region SystemRole Manager

        private void RemoveRoleListCache(int userId)
        {
            //CommonObjectCache.RemoveKey(string.Format(AuthorizationService.UserRoleCacheKey, sessionKey));
            RedisRoleAuthorizationService.RemoveRoleListCache(userId);
        }
        #endregion

        public void ClearAuthorizationCache(int userId)
        {
            RemoveFunctionListCache(userId);
            RemoveRoleListCache(userId);
        }

        public bool FunctionAuthorize(string authKey)
        {
            return FunctionAuthorize(CurrentSessionInfo.Instance.ApplicationUser.UserId, authKey);
        }

        public bool FunctionAuthorize(int userId, string permissionName)
        {
            return RedisFunctionAuthorizationServivce.FunctionAuthorize(permissionName, userId);
        }

        public bool RoleAuthorize(string roleName)
        {
            return RoleAuthorize(CurrentSessionInfo.Instance.ApplicationUser.UserId, roleName);
        }

        public bool RoleAuthorize(int userId, string roleName)
        {
            return RedisRoleAuthorizationService.RoleAuthorize(roleName, userId);
        }
    }
}
