using System.Reflection;
using Abp.EntityFramework;
using Abp.Modules;
using Abp.Reflection;
using Castle.Core.Logging;

namespace NBK.ECService.WMS.Repository
{
    [DependsOn(typeof(AbpEntityFrameworkModule))]
    public class WMSRepositoryModule: AbpModule
    {
        public ILogger Logger { get; set; }

        public WMSRepositoryModule(ITypeFinder typeFinder)
        {
            Logger = NullLogger.Instance;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}