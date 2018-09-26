using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
        public static AuthServiceReference.SystemUser Login(string loginName, string password)
        {
            AuthServiceReference.AuthServiceSoapClient authServiceSoapClient = new AuthServiceReference.AuthServiceSoapClient();
            AuthServiceReference.SystemUser authSystemUser = authServiceSoapClient.Login(loginName, password);
            
            return authSystemUser;
        }

        /// <summary>
        /// 判断当前用户在当前应用程序中是否存在此权限Key
        /// </summary>
        /// <param name="authKey">权限Key</param>
        /// <returns></returns>
        public static bool HasFunction(AuthKey authKey)
        {
            bool result = false;
            AuthServiceReference.AuthServiceSoapClient authServiceSoapClient = new AuthServiceReference.AuthServiceSoapClient();
            result = authServiceSoapClient.CheckUserFunction(SystemDataConst.ApplicationID, authKey.Key, SystemDataConst.UserName);
            return result;
        }

        /// <summary>
        /// 判断当前用户在当前应用程序中是否存在此角色Key
        /// </summary>
        /// <param name="roleKey">角色Key</param>
        /// <returns></returns>
        public static bool HasRole(RoleKey roleKey)
        {
            bool result = false;
            AuthServiceReference.AuthServiceSoapClient authServiceSoapClient = new AuthServiceReference.AuthServiceSoapClient();
            var systemRoleList = authServiceSoapClient.GetUserFunction(SystemDataConst.ApplicationID, SystemDataConst.UserName).ToList<AuthServiceReference.SystemRole>();
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
        public static List<AuthKey> GetUserAuthKey()
        {
            List<AuthKey> authKeyList = new List<AuthKey>();
            AuthServiceReference.AuthServiceSoapClient authServiceSoapClient = new AuthServiceReference.AuthServiceSoapClient();
            var result = authServiceSoapClient.GetUserAuthKey(SystemDataConst.ApplicationID, SystemDataConst.UserName);
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
        public static List<RoleKey> GetUserRoleKey()
        {
            List<RoleKey> roleKeyList = new List<RoleKey>();
            AuthServiceReference.AuthServiceSoapClient authServiceSoapClient = new AuthServiceReference.AuthServiceSoapClient();
            var result = authServiceSoapClient.GetUserFunction(SystemDataConst.ApplicationID, SystemDataConst.UserName);
            foreach (var item in result)
            {
                roleKeyList.Add(new RoleKey(item.RoleName));
            }
            return roleKeyList;
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

    public static partial class AuthKeyConst
    {
        #region [容器管理]

        public readonly static AuthKey BaseData_Container_Create = new AuthKey("BaseData_Container_Create");

        public readonly static AuthKey BaseData_Container_Update = new AuthKey("BaseData_Container_Update");

        #endregion
        
    }

    public static partial class RoleKeyConst
    {
        #region [PM]

        public readonly static RoleKey CMS_Item_PMLeader = new RoleKey("CMS_Item_PMLeader");

        #endregion
    }

    public static partial class SystemDataConst
    {
        #region [应用程序ID]

        public readonly static string ApplicationID = "d95233b8-4617-8133-885a-9a129c4ce749";

        #endregion

        #region Demo数据设置

        //实际项目中请使用一下代码获取UserName 
        //public readonly static string UserName = AppRuntime.BussinessContex.CurrentStore.UserName;

        //当前Demo 使用固定账号 Winnie 没有审核权限
        //public readonly static string UserName = "Winnie";

        //当前Demo 使用固定账号 Franky 有审核权限
        public readonly static string UserName = "wms_admin";
        #endregion
    }

    #region 辅助工具类

    public static class UtilityTools
    {
        /// <summary>
        /// 实体类复制
        /// </summary>
        /// <param name="objold"></param>
        /// <param name="objnew"></param>
        public static void EntityCopy(object objold, object objnew)
        {
            Type myType = objold.GetType(),
                myType2 = objnew.GetType();
            PropertyInfo currobj = null;
            if (myType == myType2)
            {
                PropertyInfo[] myProperties = myType.GetProperties();
                for (int i = 0; i < myProperties.Length; i++)
                {
                    currobj = objold.GetType().GetProperties()[i];
                    currobj.SetValue(objnew, currobj.GetValue(objold, null), null);
                }
            }
        }

        /// <summary>
        /// 实体类克隆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="RealObject"></param>
        /// <returns></returns>
        public static T Clone<T>(T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }
    }

    #endregion
}
