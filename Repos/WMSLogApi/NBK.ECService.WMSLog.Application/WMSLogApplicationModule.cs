using Abp.Application.Services;
using Abp.Modules;
using Abp.WebApi.Controllers.Dynamic.Builders;
using System.Reflection;
using NBK.ECService.WMSLog.Repository;

namespace NBK.ECService.WMSLog.Application
{
    [DependsOn( typeof(WMSLogRepositoryModule))]
    public class WMSLogApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Settings.Providers.Add<WMSLogSettingProvider>();
        }

        public override void Initialize()
        {
            HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            DynamicApiControllerBuilder.ForAll<IApplicationService>(typeof(WMSLogApplicationModule).Assembly, "WMS").Build();
        }
    }
}