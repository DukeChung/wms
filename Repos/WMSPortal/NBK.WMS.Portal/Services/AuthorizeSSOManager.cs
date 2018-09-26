using FortuneLab.WebApiClient;
using NBK.AuthServiceUtil;
using NBK.ECService.WMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NBK.AuthServiceUtil
{
    public static class AuthorizeSSOManager
    {

        /// <summary>
        /// 获取登录用户在当前应用程序下的所有权限Key 
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="loginName">登录名称</param>
        /// <returns></returns>
        public static ResultMessages<string> GetUserAuthKey(string token, string loginName)
        {
            var address = "api/Auth/GetUserAuthKey";
            var json = JsonConvert.SerializeObject(new { AppKey = AuthCenterConfig.AuthApplicationKey, LoginName = loginName });
            var client = HttpHelper.HttpPostAsync(AuthCenterConfig.AuthCenterUrl + address, token, json);
            var responseJson = client.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonConvert.DeserializeObject<ResultMessages<string>>(responseJson);
        }

        public static ResultMessages<string> GetUserInfo(string token)
        {
            var address = "user";
            var json = JsonConvert.SerializeObject(new { AppKey = AuthCenterConfig.AuthApplicationKey, userName = "testuser" });
            var client = HttpHelper.HttpPostAsync(AuthCenterConfig.SSOUrl + address, token, json);
            var responseJson = client.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonConvert.DeserializeObject<ResultMessages<string>>(responseJson);
        } 

        /// <summary>
        /// 检查登录用户在当前应用程序下是否存在此权限
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="loginName">登录名称</param>
        /// <param name="authKey">权限Key</param>
        /// <returns></returns>
        public static ResultMessage CheckUserFunction(string token, string loginName, string authKey)
        {
            var address = "api/Auth/CheckUserFunction";
            var json = JsonConvert.SerializeObject(new { AppKey = AuthCenterConfig.AuthApplicationKey, LoginName = loginName, AuthKey = authKey });
            var client = HttpHelper.HttpPostAsync(AuthCenterConfig.AuthCenterUrl + address, token, json);
            var responseJson = client.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonConvert.DeserializeObject<ResultMessage>(responseJson);
        }

        /// <summary>
        /// 检查用户在此应用下是否存在角色信息-供登录系统做判断处理
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="loginName">登录名称</param>
        /// <returns></returns>
        public static ResultMessage CheckUserRole(string token, string loginName)
        {
            var address = "api/Auth/CheckUserRole";
            var json = JsonConvert.SerializeObject(new { AppKey = AuthCenterConfig.AuthApplicationKey, LoginName = loginName });
            var client = HttpHelper.HttpPostAsync(AuthCenterConfig.AuthCenterUrl + address, token, json);
            var responseJson = client.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonConvert.DeserializeObject<ResultMessage>(responseJson);
        }

        /// <summary>
        /// 根据客户端得到以SSO登录的信息 
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="clientId">客户端clientId</param>
        /// <returns></returns>
        public static ResultMessages<AppSingleViewModel> GetAppByClientId(string token, string clientId)
        {
            var address = "api/SSO/GetAppByClientId";
            var json = JsonConvert.SerializeObject(new { ClientId = clientId });
            var client = HttpHelper.HttpPostAsync(AuthCenterConfig.AuthCenterUrl + address, token, json);
            var responseJson = client.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonConvert.DeserializeObject<ResultMessages<AppSingleViewModel>>(responseJson);
        }

        /// <summary>
        /// 获取SSO token 地址
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string GetSSOTokenURL(string code)
        {
            var tokenURL = "/oauth2/access_token";
            return AuthCenterConfig.UserTokenURL + tokenURL + $"?client_id={AuthCenterConfig.SSOClientID}&client_secret={AuthCenterConfig.SSOClientSecret}&grant_type=authorization_code&code=" + code;
        }

        /// <summary>
        /// 获取SSO 用户信息 地址
        /// </summary>
        /// <param name="SSOToken"></param>
        /// <returns></returns>
        private static string GetSSOUserURL(string SSOToken)
        {
            var userURL = "/user/userinfo";
            return AuthCenterConfig.UserTokenURL + userURL + "?access_token=" + SSOToken;
        }

        private static string HttpGetAsync(string url)
        {
            HttpClient httpClient = new HttpClient();
            return httpClient.GetStringAsync(url).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 获取SSO token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static SSOUserToken GetSSOToken(string code)
        {
            var tokenJson = HttpHelper.CreateGetHttpResponse(GetSSOTokenURL(code));
            SSOUserToken userToken = JsonConvert.DeserializeObject<SSOUserToken>(tokenJson);
            return userToken;
        }

        /// <summary>
        /// 获取SSO 用户信息
        /// </summary>
        /// <param name="ssoToken"></param>
        /// <returns></returns>
        public static AuthUser GetSSOUser(string ssoToken)
        {
            string ssoUserURL = GetSSOUserURL(ssoToken);
            var ssoUserJson = HttpGetAsync(ssoUserURL);
            var User = JsonConvert.DeserializeObject<AuthUser>(ssoUserJson);
            return User;
        } 

        /// <summary>
        /// 获取连接AuthCenter接口的Token
        /// </summary>
        /// <returns></returns>
        public static AuthCenterTokenViewModel GetAuthCenterToken(string userName, string password)
        {
            var address = "api/token";
            var json = JsonConvert.SerializeObject(new { UserName = userName, Password = password });
            var client = HttpHelper.HttpPostAsync(AuthCenterConfig.AuthCenterUrl + address, "", json);
            var responseJson = client.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonConvert.DeserializeObject<AuthCenterTokenViewModel>(responseJson);
        }
    }


    public class AuthCenterTokenViewModel
    {
        public Entity Entity { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

    }

    public class Entity
    {
        public string ExpiresIn { get; set; }

        public string Token { get; set; }
    }

    public class AuthUser
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public int Status { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public int Gender { get; set; }

        public bool Valid { get; set; }
    }

    public class AppSingleViewModel
    {

    }

    public class SSOUserToken
    {
        public string Access_Token { get; set; }

        public int Expires_In { get; set; }

        public string Scope { get; set; }
    }

    public class BaseResult
    {
        /// <summary>
        /// 调用返回状态编码
        /// </summary>
        public MessageCode Code { get; set; } = MessageCode.Success;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; } = "操作成功";
    }

    public enum MessageCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 验证失败
        /// </summary>
        ValidFailed = 2,
        /// <summary>
        /// 失败
        /// </summary>
        Failed = 4,
        /// <summary>
        /// 传入参数错误
        /// </summary>
        ParameterFailed = 8
    }

    public class ResultMessage : BaseResult
    {
        public object ResultVal { get; set; }
    }

    public class ResultMessages<TEntity> : BaseResult where TEntity : class
    {
        /// <summary>
        /// 数据列表
        /// </summary>
        public virtual IList<TEntity> Entities { get; set; }
    }

    public class ResultMessage<TEntity> : BaseResult where TEntity : class
    {
        public virtual TEntity Entity { get; set; }
    }


    /// <summary>
    /// AuthCenter相关配置信息
    /// </summary>
    public static class AuthCenterConfig
    { 
        public readonly static string AuthApplicationKey = ConfigurationManager.AppSettings["AuthApplicationKey"];

        public readonly static string AuthCenterUrl = ConfigurationManager.AppSettings["AuthCenterUrl"];

        public readonly static string SSOUrl = ConfigurationManager.AppSettings["SSOUrl"];

        public readonly static string UserTokenURL = ConfigurationManager.AppSettings["UserTokenURL"];

        public readonly static string AuthCenterClientID = ConfigurationManager.AppSettings["AuthCenterClientID"];

        public readonly static string AuthCenterClientSecret = ConfigurationManager.AppSettings["AuthCenterClientSecret"];

        public readonly static string SSOClientID = ConfigurationManager.AppSettings["SSOClientID"];

        public readonly static string SSOClientSecret = ConfigurationManager.AppSettings["SSOClientSecret"];

        public readonly static string RedirectURL = ConfigurationManager.AppSettings["RedirectURL"];
    }
}