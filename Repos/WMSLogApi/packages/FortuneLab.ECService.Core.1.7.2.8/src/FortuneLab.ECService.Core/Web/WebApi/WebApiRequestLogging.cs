using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using NLog;

namespace FortuneLab.ECService.Web.WebApi
{
    public static class WebApiRequestLogging
    {
        public const string DurationKey = "ExecuteDuration";
        public const string RequestTraceIdKey = "RequestTraceId";
        public const string RequestCaller = "RequestCaller";
        public const string RequestFlag = "RequestFlag";

        public static void LogRequestInfo(HttpActionExecutedContext actionExecutedContext, Stopwatch stopwatch)
        {
            //https://github.com/nlog/nlog/wiki/Event-context%20layout%20renderer Use NLog Event Context
            var theEvent = new LogEventInfo
            {
                LoggerName = "FortuneLab.Monitor.Api.Request",
                Level = actionExecutedContext.Exception == null ? LogLevel.Debug : LogLevel.Error,
                Message = actionExecutedContext.Exception == null ? "请求已正常处理" : "请求出现错误, 请查看Exception信息",
                Exception = actionExecutedContext.Exception
            };

            #region 组织请求与返回信息
            theEvent.Properties["RequestHeader"] = JsonConvert.SerializeObject(actionExecutedContext.Request.Headers);
            if (HttpContext.Current.Request.ContentLength > 0)
            {
                if (actionExecutedContext.Request.Content is MultipartContent)
                {
                    theEvent.Properties["RequestBody"] = "MultipartContent";
                }
                else
                {
                    using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
                    {
                        theEvent.Properties["RequestBody"] = $"Request ContentType:{actionExecutedContext.Request.Content.GetType().FullName}, Data:{reader.ReadToEnd().GetLimitString(20000, 20000)}";
                    }
                }
            }

            if (actionExecutedContext.Response?.Content != null)
            {
                var content = actionExecutedContext.Response.Content;
                if (content is ObjectContent)
                {
                    var oContent = (ObjectContent)content;
                    theEvent.Properties["ResponseContent"] = oContent.Value is string ? oContent.Value : JsonConvert.SerializeObject(oContent.Value).GetLimitString();
                }
                else if (content is StringContent)
                {
                    var oContent = content as StringContent;
                    theEvent.Properties["ResponseContent"] = oContent?.ReadAsStringAsync().Result.GetLimitString();
                }

                if (!theEvent.Properties.ContainsKey("ResponseContent"))
                {
                    theEvent.Properties["ResponseContent"] = $"CntentType:{actionExecutedContext.Response.Content.GetType().FullName}"; //(HttpContext.Current.Response.OutputStream.CanRead ? new StreamReader(HttpContext.Current.Response.OutputStream).ReadToEnd() : "NULL").GetLimitString();
                }
            }
            else
            {
                theEvent.Properties["ResponseContent"] = "Response Is NULL";
            }
            #endregion

            theEvent.Properties[DurationKey] = stopwatch.ElapsedMilliseconds;
            theEvent.Properties[RequestTraceIdKey] = actionExecutedContext.Request.Properties[RequestTraceIdKey];

            var actionArgs = actionExecutedContext.ActionContext.ActionArguments;
            theEvent.Properties[RequestCaller] = actionArgs.ContainsKey(RequestCaller) ? actionArgs[RequestCaller]?.ToString().Replace(".", "_").Trim() ?? "Defalut" : "Default";
            theEvent.Properties[RequestFlag] = actionArgs.ContainsKey(RequestFlag) ? actionArgs[RequestFlag]?.ToString().Replace(".", "_").Trim() ?? "Defalut" : "Default";
            LogManager.GetLogger(theEvent.LoggerName).Log(theEvent);
        }
    }

    /// <summary>
    /// 系统日志参数属性
    /// </summary>
    public class SysLogParmsAttribute : ActionFilterAttribute
    {
        public SysLogParmsAttribute()
        {

        }

        public string RequestCaller { get; set; }
        public string RequestFlag { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            actionContext.ActionArguments.Add(WebApiRequestLogging.RequestCaller, RequestCaller);
            actionContext.ActionArguments.Add(WebApiRequestLogging.RequestFlag, RequestFlag);
        }
    }
}
