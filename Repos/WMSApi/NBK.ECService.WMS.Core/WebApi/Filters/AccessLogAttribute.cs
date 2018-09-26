using Abp.Web.WebApi;
using NBK.ECService.WMS.Core.Securities;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.MQ.Log;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.RabbitMQ;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace NBK.ECService.WMS.Core.WebApi.Filters
{
    public class AccessLogAttribute : BaseActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            HttpContext.Current.Items.Add(PublicConst.AccessLogSysId, Guid.NewGuid());

            SetRequestProperty(actionContext, new WMSUserPrincipal<int>());

            base.OnActionExecuting(actionContext);
        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);

            new Task(() =>
            {
                SendMQLogInfo(actionExecutedContext);
            }).Start();
        }

        private void SendMQLogInfo(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                string controller = string.Empty;
                string service = string.Empty;
                for (int i = 0; i < actionExecutedContext.Request.RequestUri.Segments.Count(); i++)
                {
                    if (i <= 2)
                    {
                        controller += actionExecutedContext.Request.RequestUri.Segments[i];
                    }
                    else
                    {
                        service += actionExecutedContext.Request.RequestUri.Segments[i];
                    }
                }
                
                var httpContext = ((HttpContextWrapper)actionExecutedContext.Request.Properties["MS_HttpContext"]);
                var ip = GetHostAddress(httpContext);//httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                //ip = !string.IsNullOrEmpty(ip) ? ip.Split(',').SingleOrDefault() : httpContext.Request.ServerVariables["REMOTE_ADDR"];

                AccessLogDto logDto = new AccessLogDto()
                {
                    app_controller = controller,
                    app_service = service,
                    //user_id = baseDtoValue != null ? ((BaseDto)baseDtoValue).CurrentUserId.ToString() : string.Empty,
                    //user_name = baseDtoValue != null ? ((BaseDto)baseDtoValue).CurrentUserId.ToString() : string.Empty,
                    descr = string.Empty,
                    start_time = httpContext.Timestamp,
                    end_time = DateTime.Now,
                    ip = ip,
                    request_json = JsonConvert.SerializeObject(actionExecutedContext.ActionContext.ActionArguments),
                    //response_json = JsonConvert.SerializeObject(responseContent.Value),
                    flag = actionExecutedContext.Response.StatusCode == System.Net.HttpStatusCode.OK ? true : false
                };

                if (actionExecutedContext.Response != null)
                {
                    logDto.response_json = JsonConvert.SerializeObject((actionExecutedContext.Response.Content as ObjectContent).Value);
                }
                else
                {
                    logDto.response_json = JsonConvert.SerializeObject(actionExecutedContext.Exception);
                }

                var accessLogSysId = GetAccessLogSysId();
                if (accessLogSysId.HasValue)
                    logDto.SysId = accessLogSysId.Value;

                var baseDtoValue = actionExecutedContext.ActionContext.ActionArguments.FirstOrDefault(p => p.Value is BaseDto).Value;
                if (baseDtoValue != null)
                {
                    logDto.user_id = ((BaseDto)baseDtoValue).CurrentUserId.ToString();
                    logDto.user_name = ((BaseDto)baseDtoValue).CurrentDisplayName;
                }

                RabbitWMS.SetRabbitMQAsync<AccessLogDto>(Utility.Enum.RabbitMQType.AccessLog, logDto);
            }
            catch (Exception ex)
            {

            }
        }

        protected void SetRequestProperty(HttpActionContext filterContext, WMSUserPrincipal<int> principal)
        {
            var headers = HttpContext.Current.Request.Headers;
            principal.AddItem(WMSHttpHeaders.UserAgent, headers["UserAgent"]);
            principal.AddItem(WMSHttpHeaders.UserIpAddressV4, headers["UserAddr"] ?? GetClientIp(filterContext));//默认从我们的Portal获取，获取不到从当前的HttpRequest获取
            principal.AddItem(WMSHttpHeaders.IgnoreEnvelope, headers["IgnoreEnvelope"]);
            principal.AddItem(WMSHttpHeaders.AccessLogSysId, Guid.NewGuid().ToString());

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

        /// <summary>
        /// 获取客户端IP地址（无视代理）
        /// </summary>
        /// <returns>若失败则返回回送地址</returns>
        public static string GetHostAddress(HttpContextWrapper httpContext)
        {
            string userHostAddress = httpContext.Request.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrEmpty(userHostAddress))
            {
                if (httpContext.Request.ServerVariables["HTTP_VIA"] != null)
                    userHostAddress = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString().Split(',')[0].Trim();
            }
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = httpContext.Request.UserHostAddress;
            }

            //最后判断获取是否成功，并检查IP地址的格式（检查其格式非常重要）
            if (!string.IsNullOrEmpty(userHostAddress) && IsIP(userHostAddress))
            {
                return userHostAddress;
            }
            return "127.0.0.1";
        }

        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }



        private Guid? GetAccessLogSysId()
        {
            Guid sysId = Guid.Empty;
            if (Guid.TryParse((Thread.CurrentPrincipal as WMSUserPrincipal<int>).GetItem<string>(WMSHttpHeaders.AccessLogSysId), out sysId))
            {
                return sysId;
            }
            return null;
        }
    }
}
