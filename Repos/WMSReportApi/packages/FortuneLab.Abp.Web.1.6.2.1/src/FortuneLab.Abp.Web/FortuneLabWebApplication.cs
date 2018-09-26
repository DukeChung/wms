using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Abp.Dependency;
using Abp.Web;
using Castle.Facilities.Logging;

namespace FortuneLab.Web
{
    public class FortuneLabWebApplication : AbpWebApplication
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            IocManager.Instance.IocContainer.AddFacility<LoggingFacility>(f => f.UseNLog().WithConfig("NLog.config"));
            base.Application_Start(sender, e);
            IocManager.Instance.Resolve<Abp.Auditing.IAuditingConfiguration>().IsEnabled = false;

            GlobalConfiguration.Configuration.EnsureInitialized();
        }
    }
}
