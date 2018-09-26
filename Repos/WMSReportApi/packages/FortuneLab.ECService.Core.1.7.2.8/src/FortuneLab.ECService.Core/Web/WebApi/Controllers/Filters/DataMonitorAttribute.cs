using Abp.Securities;
using FortuneLab;
using FortuneLab.WebApiClient;
using Newtonsoft.Json;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using FortuneLab.ECService.Web.WebApi;

namespace Abp.Web.WebApi.Controllers.Filters
{
    public class DataMonitorAttribute : BaseActionFilterAttribute
    {
        public const string StopwatchKey = "StopwatchFilter.Value";
        public const string DurationKey = "ExecuteDuration";
        public const string RequestTraceIdKey = "RequestTraceId";
        public const string RequestCaller = "RequestCaller";
        public const string RequestFlag = "RequestFlag";

        public bool IsLoggerEnable { get; set; }

        public DataMonitorAttribute()
        {
            this.Order = -99;//最早执行，最后退出
            this.IsLoggerEnable = true;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var isStreamContent = actionContext.Request.Content.IsMimeMultipartContent("form-data");//actionContext.Request.Content.Headers.ContentType.MediaType.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase);
            if (!isStreamContent)
            {
                actionContext.Request.Content.ReadAsStringAsync().Wait();//确保数据已完全读取，避免InputStream读取的时候报错误

                if (HttpContext.Current.Request.InputStream.CanRead)
                    HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            var traceId = actionContext.Request.Headers.Contains(RequestTraceIdKey)
                ? actionContext.Request.Headers.GetValues(RequestTraceIdKey).FirstOrDefault()
                : GetNextRequestTraceId();

            actionContext.Request.Properties[StopwatchKey] = stopwatch;
            actionContext.Request.Properties[RequestTraceIdKey] = traceId;
            actionContext.Request.Properties[RequestCaller] = "Unknow";
            actionContext.Request.Properties[RequestFlag] = "Unknow";
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            Stopwatch stopwatch = actionExecutedContext.Request.Properties[StopwatchKey] as Stopwatch;

            if (actionExecutedContext.Response != null && stopwatch != null)
            {
                actionExecutedContext.Response.Headers.Add("Duration", stopwatch.ElapsedMilliseconds.ToString());
            }
            if (IsLoggerEnable && actionExecutedContext.Exception == null)
            {
                WebApiRequestLogging.LogRequestInfo(actionExecutedContext, stopwatch);
            }
        }

        //private static void LogResponseInfo(HttpActionExecutedContext actionExecutedContext, Stopwatch stopwatch)
        //{
        //    //https://github.com/nlog/nlog/wiki/Event-context%20layout%20renderer Use NLog Event Context
        //    LogEventInfo theEvent = new LogEventInfo();
        //    theEvent.Level = LogLevel.Debug;

        //    theEvent.Properties["RequestHeader"] = JsonConvert.SerializeObject(actionExecutedContext.Request.Headers);
        //    if (actionExecutedContext.Request.Content is MultipartContent)
        //    {
        //        theEvent.Properties["RequestBody"] = "MultipartContent";
        //    }
        //    else
        //    {
        //        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
        //        {
        //            theEvent.Properties["RequestBody"] = reader.ReadToEnd().GetLimitString(20000, 20000);
        //        }
        //    }

        //    if (actionExecutedContext.Response?.Content != null)
        //    {
        //        if (actionExecutedContext.Response.Content is ObjectContent)
        //        {
        //            theEvent.Properties["ResponseContent"] = JsonConvert.SerializeObject(((ObjectContent)actionExecutedContext.Response.Content).Value ?? "").GetLimitString();
        //        }
        //        else if (actionExecutedContext.Response.Content is StreamContent)
        //        {
        //            theEvent.Properties["ResponseContent"] = "StreamContent";
        //        }
        //        else if (actionExecutedContext.Response.Content is HttpContext)
        //        {
        //            //theEvent.Properties["ResponseContent"] = ((HttpContext)actionExecutedContext.Response.Content).Response.OutputStream
        //        }
        //        else
        //        {
        //            theEvent.Properties["ResponseContent"] = (HttpContext.Current.Response.OutputStream.CanRead ? new StreamReader(HttpContext.Current.Response.OutputStream).ReadToEnd() : "NULL").GetLimitString();
        //        }
        //    }
        //    else
        //    {
        //        theEvent.Properties["ResponseContent"] = "Response Is NULL";
        //    }
        //    theEvent.LoggerName = "FortuneLab.Monitor.Api.Response";
        //    theEvent.Properties[DurationKey] = stopwatch.ElapsedMilliseconds;
        //    theEvent.Properties[RequestTraceIdKey] = actionExecutedContext.Request.Properties[RequestTraceIdKey];
        //    theEvent.Properties[RequestCaller] = actionExecutedContext.Request.Properties[RequestCaller];
        //    theEvent.Properties[RequestFlag] = actionExecutedContext.Request.Properties[RequestFlag];
        //    LogManager.GetLogger(theEvent.LoggerName).Log(theEvent);
        //}

        private string GetNextRequestTraceId()
        {
            var r = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
            return $"{DateTime.Now:yyyyMMddHHmmss}{r.Next(100000, 999999)}";
        }
    }

    public class PlatformUser : IFortuneLabUser<int>
    {
        public PlatformUser(int userId)
        {
            this.UserId = userId;
        }

        public int UserId { get; private set; }

        public string LoginName { get; private set; }

        public string DisplayName { get; private set; }
    }
}
