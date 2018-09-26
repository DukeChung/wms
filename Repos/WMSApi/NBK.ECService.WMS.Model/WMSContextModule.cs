using System.Reflection;
using Abp.Modules;

namespace NBK.ECService.WMS.Model.Models
{
    public class WMSContextModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            //DynamicApiControllerBuilder.ForAll<IApplicationService>(typeof(LandRoverApplicationModule).Assembly, "LandRover").Build();
        }
    }
}