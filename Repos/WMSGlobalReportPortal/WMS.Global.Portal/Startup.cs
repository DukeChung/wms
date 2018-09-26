using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WMS.Global.Portal.Startup))]
namespace WMS.Global.Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}