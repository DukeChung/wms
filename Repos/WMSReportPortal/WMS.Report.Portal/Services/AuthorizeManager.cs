using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.Report.Portal.AuthServiceReference;

namespace NBK.AuthServiceUtil
{
    /// <summary>
    /// 授权管理
    /// </summary>
    public static class AuthorizeManager
    {
        /// <summary>
        /// 用户登录验证
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="password">密码</param>
        /// <returns>AuthCenter用户信息</returns>
        public static SystemUser Login(string loginName, string password)
        {
            AuthServiceSoapClient authServiceSoapClient = new AuthServiceSoapClient();
            SystemUser authSystemUser = authServiceSoapClient.LoginAuth(loginName, password);

            return authSystemUser;
        }

        /// <summary>
        /// 判断当前用户在当前应用程序中是否存在此权限Key
        /// </summary>
        /// <param name="authKey">权限Key</param>
        /// <returns></returns>
        public static bool HasFunction(AuthKey authKey, string loginUserName)
        {
            bool result = false;
            AuthServiceSoapClient authServiceSoapClient = new AuthServiceSoapClient();
            result = authServiceSoapClient.CheckUserFunction(SystemDataConst.ApplicationID, authKey.Key, loginUserName);
            return result;
        }

        /// <summary>
        /// 判断当前用户在当前应用程序中是否存在此角色Key
        /// </summary>
        /// <param name="roleKey">角色Key</param>
        /// <returns></returns>
        public static bool HasRole(RoleKey roleKey, string loginUserName)
        {
            bool result = false;
            AuthServiceSoapClient authServiceSoapClient = new AuthServiceSoapClient();
            var systemRoleList = authServiceSoapClient.GetUserFunction(SystemDataConst.ApplicationID, loginUserName).ToList<SystemRole>();
            if (systemRoleList != null && systemRoleList.Count > 0)
            {
                result = systemRoleList.Find(x => { return x.RoleName == roleKey.Key; }) != null;
            }
            return result;
        }

        /// <summary>
        /// 获取当权用户在当前应用程序中的所有权限Key
        /// </summary>
        /// <returns></returns>
        public static List<AuthKey> GetUserAuthKey(string loginUserName)
        {
            List<AuthKey> authKeyList = new List<AuthKey>();
            AuthServiceSoapClient authServiceSoapClient = new AuthServiceSoapClient();
            var result = authServiceSoapClient.GetUserAuthKey(SystemDataConst.ApplicationID, loginUserName);
            foreach (var item in result)
            {
                authKeyList.Add(new AuthKey(item));
            }
            return authKeyList;
        }

        /// <summary>
        /// 获取当权用户在当前应用程序中的所有角色Key
        /// </summary>
        /// <returns></returns>
        public static List<RoleKey> GetUserRoleKey(string loginUserName)
        {
            List<RoleKey> roleKeyList = new List<RoleKey>();
            AuthServiceSoapClient authServiceSoapClient = new AuthServiceSoapClient();
            var result = authServiceSoapClient.GetUserFunction(SystemDataConst.ApplicationID, loginUserName);
            foreach (var item in result)
            {
                roleKeyList.Add(new RoleKey(item.RoleName));
            }
            return roleKeyList;
        }
    }

    /// <summary>
    /// 权限授权验证
    /// </summary>
    public class PermissionAuthorizeAttribute : ActionFilterAttribute
    {
        public List<string> RequiredPermissionNames { get; private set; }

        public PermissionAuthorizeAttribute(params string[] requiredPermissions)
        {
            this.RequiredPermissionNames = new List<string>();
            this.RequiredPermissionNames.AddRange(requiredPermissions);
            this.Order = 1;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = filterContext.HttpContext.User.Identity.Name;

            if (!AuthorizeManager.HasFunction(new AuthKey(RequiredPermissionNames.FirstOrDefault()), user))
            {
                filterContext.Result = UnAuthorizedResult();
            }
        }

        public ActionResult UnAuthorizedResult()
        {
            return new RedirectResult("/Account/AccessDenied");
        }
    }

    public class AuthKey
    {
        public AuthKey(string key)
        {
            if (key == null || key.Trim().Length <= 0)
            {
                throw new Exception("The auth key cannot be null or empty!");
            }
            Key = key;
        }

        public string Key
        {
            get;
            private set;
        }
    }

    public class RoleKey
    {
        public RoleKey(string key)
        {
            if (key == null || key.Trim().Length <= 0)
            {
                throw new Exception("The auth key cannot be null or empty!");
            }
            Key = key;
        }

        public string Key
        {
            get;
            private set;
        }
    }

    public class AuthKeyConst
    {

    }

    public static class RoleKeyConst
    {
        #region [PM]

        public readonly static RoleKey CMS_Item_PMLeader = new RoleKey("CMS_Item_PMLeader");

        #endregion
    }

    public class SystemDataConst
    {
        #region [应用程序ID]

        public readonly static string ApplicationID = ConfigurationManager.AppSettings["AuthCenterApplicationID"];

        #endregion

        public static string LoginUser = "LoginUser";

        public static string SessionLoginUserName = "LoginUserName";

        public static string SessionLoginDisplayUserName = "LoginDisplayUserName";

        #region Demo数据设置

        //实际项目中请使用一下代码获取UserName 
        //public static string UserName = ApplicationUser

        //当前Demo 使用固定账号 Winnie 没有审核权限
        //public readonly static string UserName = "Winnie";

        //当前Demo 使用固定账号 Franky 有审核权限
        //public readonly static string UserName = "wms_admin";
        #endregion
    }
}