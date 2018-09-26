using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FortuneLab.WebClient.Mvc.Filters
{
    public class ExceptionFilterAttribute : HandleErrorAttribute
    {
        private const string RequestStopwatchKey = "RequestStopwatch";

        public override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = false;
            var sw = HttpContext.Current.Items[RequestStopwatchKey] as Stopwatch;
            WebRequestLogging.LogRequestInfo(filterContext.Exception, sw);
        }
    }
}
