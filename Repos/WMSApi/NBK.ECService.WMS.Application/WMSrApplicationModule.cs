using Abp.Application.Services;
using Abp.Modules;
using Abp.WebApi.Controllers.Dynamic.Builders;
using System.Reflection;

using NBK.ECService.WMS.Repository;

namespace NBK.ECService.WMS.Application
{
    [DependsOn( typeof(WMSRepositoryModule))]
    public class WMSrApplicationModule: AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Settings.Providers.Add<WMSSettingProvider>();
            //Configuration.Authorization.Providers.Add<ProductAuthorizationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            IocManager.Register<IMqEventBus, MqEventBus>(Abp.Dependency.DependencyLifeStyle.Transient);
            DynamicApiControllerBuilder.ForAll<IApplicationService>(typeof(WMSrApplicationModule).Assembly, "WMS").Build();
        }
    }
}