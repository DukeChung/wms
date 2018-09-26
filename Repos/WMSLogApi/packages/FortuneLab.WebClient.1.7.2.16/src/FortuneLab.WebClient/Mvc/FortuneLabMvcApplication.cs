using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FortuneLab.WebClient.Mvc.Filters;
using NLog;
using FortuneLab.Logging.TargetExtension;
using Microsoft.Owin;

namespace FortuneLab.WebClient.Mvc
{
    public class FortuneLabMvcApplication : HttpApplication
    {
        const string COOKIE_YMCAID = "ymcaid";
        const string COOKIE_YMCSID = "ymcsid";

        public FortuneLabMvcApplication()
        {
            this.BeginRequest += FortuneLabMvcApplication_BeginRequest;
        }

        private void FortuneLabMvcApplication_BeginRequest(object sender, EventArgs e)
        {
            //container.RegisterPerWebRequest(() =>
            //{
            //if (HttpContext.Current != null && HttpContext.Current.Items["owin.Environment"] == null)
            //{
            //    return new OwinContext().Authentication;
            //}
            //return HttpContext.Current.GetOwinContext().Authentication;

            //});

            var ymcaidCookie = Request.Cookies[COOKIE_YMCAID];//YMC (ERP) Application Id(所有erp系统用一个)
            if (ymcaidCookie == null || string.IsNullOrWhiteSpace(ymcaidCookie[COOKIE_YMCAID]))
            {
                Response.Cookies.Add(new HttpCookie(COOKIE_YMCAID)
                {
                    Expires = DateTime.MaxValue,
                    Value = Guid.NewGuid().ToString(),
                    Domain = System.Configuration.ConfigurationManager.AppSettings["CookieDomain"]
                });
            }

            var ymcsidCookie = Request.Cookies[COOKIE_YMCSID];//YMC (ERP) Application Id(所有erp系统用一个)
            if (ymcaidCookie == null || string.IsNullOrWhiteSpace(ymcaidCookie[COOKIE_YMCSID]))
            {
                Response.Cookies.Add(new HttpCookie(COOKIE_YMCSID)
                {
                    Value = Guid.NewGuid().ToString(),
                    Domain = System.Configuration.ConfigurationManager.AppSettings["CookieDomain"]
                });
            }
        }

        protected virtual void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            //注册框架级别的ActionFilter
            FrameworkConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            //注册NLog日志组件, 全部日志固定输出到RabbitMQ and Log2Console
            LogManager.Configuration.ConfigRabbitMQTarget();
        }

        //protected void Application_Error(Object sender, EventArgs e)
        //{
        //    var webHelper = new WebHelper(new HttpContextWrapper(System.Web.HttpContext.Current));
        //    if (webHelper.IsStaticResource(this.Request)) return;

        //    //if (Request.IsLocal)
        //    //{
        //    //    return;
        //    //}
        //    var exception = Server.GetLastError();
        //    Response.Clear();
        //    Response.TrySkipIisCustomErrors = true;
        //    //log error
        //    //LogException(exception);

        //    //process 404 HTTP errors
        //    var httpException = exception as HttpException;
        //    if (httpException != null)
        //    {
        //        // Call target Controller and pass the routeData.
        //        IController errorController = new CommonController(); //EngineContext.Current.Resolve<CommonController>();
        //        var routeData = new RouteData();
        //        routeData.Values.Add("controller", "Common");
        //        switch (httpException.GetHttpCode())
        //        {
        //            case 404:
        //                routeData.Values.Add("action", "Http404ErrorHandler");
        //                break;
        //            default:
        //                routeData.Values.Add("action", "HttpOtherErrorHandler");
        //                break;
        //        }
        //        errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        //    }
        //    else
        //    {
        //        IController errorController = new CommonController(); //EngineContext.Current.Resolve<CommonController>();
        //        var routeData = new RouteData();
        //        routeData.Values.Add("controller", "Common");
        //        routeData.Values.Add("action", "ServerErrorHandler");
        //        errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        //    }

        //    //留在最后清理是因为可以在PageNotFoundErrorHandler Action中通过Server.GetLastError()拿到异常信息
        //    while (Server.GetLastError() != null)
        //        Server.ClearError();
        //}
    }

    public static class FrameworkConfig
    {
        internal static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ExceptionFilterAttribute());
            filters.Add(new ActionTrackFilterAttribute());
        }
    }
}
