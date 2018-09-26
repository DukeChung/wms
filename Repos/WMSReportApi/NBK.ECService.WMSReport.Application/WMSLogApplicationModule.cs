using Abp.Application.Services;
using Abp.Modules;
using Abp.WebApi.Controllers.Dynamic.Builders;
using System.Reflection;
using NBK.ECService.WMSReport.Repository;

namespace NBK.ECService.WMSReport.Application
{
    [DependsOn( typeof(WMSReportRepositoryModule))]
    public class WMSReportApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Settings.Providers.Add<WMSReportSettingProvider>();
        }

        public override void Initialize()
        {
            HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            DynamicApiControllerBuilder.ForAll<IApplicationService>(typeof(WMSReportApplicationModule).Assembly, "WMS").Build();
        }
    }
}