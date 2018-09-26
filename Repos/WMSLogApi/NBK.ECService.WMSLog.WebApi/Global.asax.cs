using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FortuneLab.Web;

namespace NBK.ECService.WMS.WebApi
{
    public class WebApiApplication : FortuneLabWebApplication
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);
            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            //ISO (default): "ReleaseDate": "1901-01-08T04:00:00",
            //MicrosoftDateFormat:"ReleaseDate": "/Date(-2176862400000+0800)/",
            json.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
            //all datetime translate to GMT/UTC
            json.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Unspecified;
        }
    }
}
