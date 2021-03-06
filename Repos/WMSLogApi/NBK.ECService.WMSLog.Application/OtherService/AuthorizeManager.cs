﻿using NBK.ECService.WMSLog.Application.AuthServiceReference;
using NBK.ECService.WMSLog.DTO.Authorize;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Application.OtherService
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
            SystemUser authSystemUser = authServiceSoapClient.Login(loginName, password);

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

        public static List<ApplicationUser> GetAllSystemUser()
        {
            AuthServiceSoapClient authServiceSoapClient = new AuthServiceSoapClient();
            var result = authServiceSoapClient.GetSystemUserByApplicationID(SystemDataConst.ApplicationID);
            List<ApplicationUser> list = new List<ApplicationUser>();
            foreach (var item in result)
            {
                list.Add(new ApplicationUser(item));
            }
            return list;
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

    public class SystemDataConst
    {
        #region [应用程序ID]

        public readonly static string ApplicationID = ConfigurationManager.AppSettings["AuthCenterApplicationID"];

        #endregion

        public static string LoginUser = "LoginUser";

        public static string SessionLoginUserName = "LoginUserName";

        public static string SessionLoginDisplayUserName = "LoginDisplayUserName";

        public static string SessionLoginUserID = "LoginUserID";

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
