using FortuneLab.WebClient.Models;
using FortuneLab.WebClient.Service.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Configuration;

namespace FortuneLab.WebClient.Common
{
    public static class TrafficMonitor
    {
        public static int AppId { get; private set; }
        public static string[] IgnoreUa = new string[2] { "DNSPod", "www.monitor.us" };
        public static string[] IgnoreCookieName = new string[2] { "gmc", "__RequestVerificationToken" };

        static TrafficMonitor()
        {
            int appId;
            AppId = int.TryParse(WebConfigurationManager.AppSettings["AppId"], out appId) ? appId : 0;
        }

        public static void MonitorRequestHandler(object sender, EventArgs e)
        {
            Request(HttpContext.Current);
        }

        public static void Request(HttpContext httpContext)
        {
            Request(httpContext.Request, httpContext.Response);
        }

        public static void Request(HttpRequest httpRequest, HttpResponse httpResponse)
        {
            var webHelper = new WebHelper(new HttpContextWrapper(HttpContext.Current));
            if (webHelper.IsStaticResource(HttpContext.Current.Request)) return;

            var userAgent = httpRequest.UserAgent;

            if (IgnoreUa.Any(ignoreUa => userAgent != null && userAgent.IndexOf(ignoreUa, StringComparison.Ordinal) != -1))
            {
                return;
            }

            var cidCookie = httpRequest.Cookies["jjd"];
            Guid gcid;
            if (cidCookie == null || string.IsNullOrWhiteSpace(cidCookie.Value) || !Guid.TryParse(cidCookie.Value, out gcid))
            {
                gcid = Guid.NewGuid();
                httpResponse.Cookies.Add(new HttpCookie("jjd", gcid.ToString()) { Expires = DateTime.MaxValue.ToUniversalTime(), HttpOnly = false });
            }
            WriteLog(httpRequest, gcid);
        }

        private static void WriteLog(HttpRequest request, Guid gcid)
        {
            var cookies = new List<CookieInfo>();

            for (var i = 0; i < request.Cookies.Count; i++)
            {
                var cookie = request.Cookies[i];
                if (IgnoreCookieName.Any(cookieName => cookie != null && cookie.Name.IndexOf(cookieName, StringComparison.Ordinal) != -1))
                {
                    continue;
                }
                cookies.Add(new CookieInfo(cookie));
            }

            //var log = new TrafficRecord(AppId)
            //{
            //    Cid = gcid,
            //    Language = JsonConvert.SerializeObject(request.UserLanguages),
            //    ReferrerUrl = request.UrlReferrer == null ? "" : request.UrlReferrer.AbsoluteUri,
            //    UserAgent = request.UserAgent,
            //    Cookie = JsonConvert.SerializeObject(cookies)
            //};

            //Utility.EatException(() => { log.RequestIP = request.UserHostAddress; });
            //Utility.EatException(() => { log.RequestUrl = request.Url.AbsoluteUri; });

            //ThreadPool.QueueUserWorkItem(o =>
            //{
            //    try
            //    {
            //        //为防止页面请求速度减慢，这里采取用线程池另开启线程来记录, 如果访问量大的时候，会导致线程资源消耗严重，后面可以考虑通过队列来入库，目前先这样 Flyear/201406301710
            //        MonitorApiClient.TrafficLog(log);
            //    }
            //    catch (Exception ex)
            //    {
            //        Utility.EatException(() =>
            //        {
            //            MonitorApiClient.SiteLog(new SiteLog(AppId) { Message = string.Format("Exception Type: {0} Message: {1}", ex.GetType().FullName, ex.FullMessage()), StackTrace = ex.FullStackTrace() });
            //        });
            //    }
            //});
        }

        public class CookieInfo
        {
            public CookieInfo(HttpCookie cookie)
            {
                this.Name = cookie.Name;
                this.Value = cookie.Value;
            }

            public string Name { get; set; }

            public string Value { get; set; }
        }
    }
}