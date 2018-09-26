using Abp.Dependency;
using Abp.Securities;
using Abp.Web.WebApi;
using FortuneLab.ECService.Securities.Entities;
using FortuneLab.Exceptions;
using FortuneLab.Runtime;
using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using FortuneLab.Utils;

namespace FortuneLab.ECService.Securities.Filters
{
    public class SessionValidateAttribute : BaseActionFilterAttribute
    {
        const int SessionTimeout = 60;
        private IUserAppService _userService;
        private bool IsBackDoorEnable = false;

        public SessionValidateAttribute()
        {
            Order = 2;
            bool.TryParse(ConfigurationManager.AppSettings["api:BackDoorEnable"] ?? "false", out IsBackDoorEnable);
        }

        private ApiToken VerifyToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new FortuneLabException("Require_ApiToken", "ApiToken not provided.");
            }
            var t = System.Web.HttpRuntime.Cache["TOKEN_" + token] as ApiToken;
            if (t == null)
            {
                this._userService = IocManager.Instance.Resolve<IUserAppService>();
                t = _userService.GetToken(token);
                if (null == t)
                {
                    throw new FortuneLabException("Invalid_ApiToken", "Invalid ApiToken.");
                }
                System.Web.HttpRuntime.Cache.Add("TOKEN_" + token, t, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
            }
            return t;
        }

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            SetRequestTraceId(filterContext);

            if (filterContext.ActionArguments.ContainsKey(PublicAttribute.KeyName))
            {
                //公共API无需检查Token，直接执行
                base.OnActionExecuting(filterContext);
                SetRequestProperty(filterContext, new FortuneLabUserPrincipal<int>(new PublicUser(1, "Public"), string.Empty));//设置未登录的Principal
                return;
            }

            //检查APIToken
            var qs = HttpUtility.ParseQueryString(filterContext.Request.RequestUri.Query);
            var apiToken = VerifyToken(qs["APITOKEN"]);
            filterContext.ControllerContext.RouteData.Values[TokenName] = apiToken;

            if (filterContext.ActionArguments.ContainsKey(AnonymousAttribute.KeyName))
            {
                //匿名API在检查过Token之后跳过用户检查
                base.OnActionExecuting(filterContext);
                SetRequestProperty(filterContext, new FortuneLabUserPrincipal<int>(new PublicUser(1, "Anonymous"), string.Empty), apiToken);//设置未登录的Principal
                return;
            }

            var sessionKey = (qs[SessionKeyName] ?? string.Empty).Trim();
            this._userService = IocManager.Instance.Resolve<IUserAppService>();

            if (string.IsNullOrEmpty(sessionKey))
            {
                throw new FortuneLabException("Required_Session", "SessionKey未提供");
            }

            //backdoor
            if (IsBackDoorEnable && sessionKey.Equals("111", StringComparison.OrdinalIgnoreCase))
            {
                var logonUser = _userService.GetUserByLoginId("admin");
                filterContext.ControllerContext.RouteData.Values[LogonUserName] = logonUser;
                SetRequestProperty(filterContext, new FortuneLabUserPrincipal<int>(logonUser, "111"), apiToken);
                return;
            }
            else if (sessionKey.Length <= 16)
            {
                //长度<=16意思SessionKey是系统设定的SessionKey
                var logonUser = _userService.GetUserBySystemSession(sessionKey);
                if (logonUser == null)
                {
                    throw new FortuneLabException("Invalid_SystemSessionKey", $"SessionKey:{sessionKey} is Not a valid system SessionKey");
                }
                filterContext.ControllerContext.RouteData.Values[LogonUserName] = logonUser;
                SetRequestProperty(filterContext, new FortuneLabUserPrincipal<int>(logonUser, sessionKey), apiToken);
                //针对系统SessionKey忽略过期检查
                return;
            }

            //validate user session
            var userSession = _userService.GetUserDevice(sessionKey);

            if (userSession == null)
            {
                throw new FortuneLabException("Invalid_sessionKey", "Invalid SessionKey");
            }
            else
            {
                if (userSession.ExpiredTime < DateTime.UtcNow)
                    throw new FortuneLabException("SessionTimeOut", "SessionKey expired");

                //get user and return
                var logonUser = _userService.GetUser(userSession.UserSysId);
                if (logonUser == null)
                {
                    throw new FortuneLabException("Invalid_User", "User not found");
                }
                else
                {
                    filterContext.ControllerContext.RouteData.Values[LogonUserName] = logonUser;
                    SetRequestProperty(filterContext, new FortuneLabUserPrincipal<int>(logonUser, sessionKey), apiToken);
                }

                userSession.ActiveTime = DateTime.UtcNow;
                userSession.ExpiredTime = DateTime.UtcNow.AddMinutes(SessionTimeout);
                _userService.UpdateUserDevice(userSession);
            }

            base.OnActionExecuting(filterContext);
        }

        private static void SetRequestTraceId(HttpActionContext filterContext)
        {
            var headers = HttpContext.Current.Request.Headers;
            var traceId = headers[RequestTraceIdManager.RequestTraceIdKey];
            if (string.IsNullOrWhiteSpace(traceId))
                traceId = RequestTraceIdManager.GetNextRequestTraceId();

            HttpContext.Current.Items.Add(RequestTraceIdManager.RequestTraceIdKey, traceId);
        }


        protected void SetRequestProperty(HttpActionContext filterContext, FortuneLabUserPrincipal<int> principal, ApiToken apiToken = null)
        {
            var headers = HttpContext.Current.Request.Headers;
            principal.AddItem(FortuneLabHttpHeaders.UserAgent, headers["UserAgent"]);
            principal.AddItem(FortuneLabHttpHeaders.UserIpAddressV4, headers["UserAddr"] ?? GetClientIp(filterContext));//默认从我们的Portal获取，获取不到从当前的HttpRequest获取
            principal.AddItem(FortuneLabHttpHeaders.IgnoreEnvelope, headers["IgnoreEnvelope"]);
            principal.AddItem(FortuneLabHttpHeaders.ApiToken, apiToken);

            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        private static string GetClientIp(HttpActionContext filterContext)
        {
            var myRequest = ((HttpContextWrapper)filterContext.Request.Properties["MS_HttpContext"]).Request;
            var ip = myRequest.ServerVariables["HTTP_X_FORWARDED_FOR"];
            return !string.IsNullOrEmpty(ip) ? ip.Split(',').SingleOrDefault() : myRequest.ServerVariables["REMOTE_ADDR"];
        }
    }

    public class PublicUser : IFortuneLabUser<int>
    {
        public PublicUser(int userId, string loginName)
        {
            this.UserId = userId;
            this.LoginName = loginName;
            this.DisplayName = loginName;
        }
        public int UserId { get; }
        public string LoginName { get; }
        public string DisplayName { get; }
    }
}
