using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NBK.WMS.Portal.Startup))]
namespace NBK.WMS.Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}