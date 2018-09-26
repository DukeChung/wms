using FortuneLab;
using FortuneLab.Utils;
using NLog;
using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;


namespace FortuneLab.WebClient.Mvc.Filters
{

    public class ActionTrackFilterAttribute : ActionFilterAttribute
    {
        private const string YmcRsdtCookieName = "ymc-rsdt";
        private const string RequestStopwatchKey = "RequestStopwatch";
        private const string DurationKey = "ExecuteDuration";
        private const string RequestCaller = "RequestCaller";
        private const string RequestFlag = "RequestFlag";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;

            if (HttpContext.Current.Items.Contains(RequestStopwatchKey))
            {
                HttpContext.Current.Items.Remove(RequestStopwatchKey);
            }

            HttpContext.Current.Items.Add(RequestStopwatchKey, Stopwatch.StartNew());
            HttpContext.Current.Items.Add(RequestCaller, "DefaultCaller");
            HttpContext.Current.Items.Add(RequestFlag, "DefaultFlag");

            if (!HttpContext.Current.Items.Contains(RequestTraceIdManager.RequestTraceIdKey))
                HttpContext.Current.Items.Add(RequestTraceIdManager.RequestTraceIdKey, RequestTraceIdManager.GetNextRequestTraceId());
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;

            #region 服务器端执行时间统计
            var sw = HttpContext.Current.Items[RequestStopwatchKey] as Stopwatch;
            if (sw != null)
            {
                filterContext.HttpContext.Response.AddHeader(DurationKey, sw.ElapsedMilliseconds.ToString());
            }

            if (filterContext.Exception == null)
                WebRequestLogging.LogRequestInfo(filterContext.Exception, sw);
            #endregion

            #region 转发客户端传过来的开始时间
            if (!string.IsNullOrWhiteSpace(filterContext.HttpContext.Request.Headers[YmcRsdtCookieName]))
            {
                filterContext.HttpContext.Response.AddHeader(YmcRsdtCookieName, filterContext.HttpContext.Request.Headers[YmcRsdtCookieName]);
            }
            #endregion
        }

        //private static void LogResponseInfo(ResultExecutedContext actionExecutedContext, Stopwatch stopwatch)
        //{
        //    LogEventInfo theEvent = new LogEventInfo
        //    {
        //        Level = LogLevel.Debug,
        //        LoggerName = "FortuneLab.Monitor.Portal.Request"
        //    };
        //    //theEvent.Message = actionExecutedContext.RequestContext.HttpContext.Request.Url.AbsoluteUri;
        //    theEvent.Properties[DurationKey] = stopwatch.ElapsedMilliseconds;
        //    theEvent.Properties[RequestTraceIdManager.RequestTraceIdKey] = HttpContext.Current.Items[RequestTraceIdManager.RequestTraceIdKey];
        //    theEvent.Properties[RequestCaller] = HttpContext.Current.Items[RequestCaller];
        //    theEvent.Properties[RequestFlag] = HttpContext.Current.Items[RequestFlag];
        //    LogManager.GetLogger(theEvent.LoggerName).Log(theEvent);
        //}
    }

    public static class WebRequestLogging
    {
        public const string DurationKey = "ExecuteDuration";
        public const string RequestCaller = "RequestCaller";
        public const string RequestFlag = "RequestFlag";

        public static void LogRequestInfo(Exception exception, Stopwatch stopwatch)
        {
            //https://github.com/nlog/nlog/wiki/Event-context%20layout%20renderer Use NLog Event Context
            var theEvent = new LogEventInfo
            {
                LoggerName = "FortuneLab.Monitor.Portal.Request",
                Level = exception == null ? LogLevel.Debug : LogLevel.Error,
                Message = exception == null ? "请求已正常处理" : exception.FullMessage(),
                Exception = exception
            };

            theEvent.Properties[DurationKey] = stopwatch?.ElapsedMilliseconds.ToString();
            theEvent.Properties[RequestTraceIdManager.RequestTraceIdKey] = HttpContext.Current.Items[RequestTraceIdManager.RequestTraceIdKey];
            theEvent.Properties[RequestCaller] = HttpContext.Current.Items[RequestCaller];
            theEvent.Properties[RequestFlag] = HttpContext.Current.Items[RequestFlag];

            LogManager.GetLogger(theEvent.LoggerName).Log(theEvent);
        }
    }
}
