using System.Reflection;
using Abp.Modules;

namespace NBK.ECService.WMS.ApiController
{
    public class WMSApiControllerModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Configuration.Settings.Providers.Add<LandRoverSettingProvider>();
            //Configuration.Authorization.Providers.Add<ProductAuthorizationProvider>();
        }

        public override void Initialize()
        {
            HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            //DynamicApiControllerBuilder.ForAll<IApplicationService>(typeof(LandRoverApplicationModule).Assembly, "LandRover").Build();
        }
    }
}