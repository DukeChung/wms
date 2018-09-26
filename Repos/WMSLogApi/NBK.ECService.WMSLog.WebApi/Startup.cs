using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(NBK.ECService.WMSLog.WebApi.Startup))]

namespace NBK.ECService.WMSLog.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            app.MapSignalR(new HubConfiguration() { EnableJSONP = true });
        }
    }
}
