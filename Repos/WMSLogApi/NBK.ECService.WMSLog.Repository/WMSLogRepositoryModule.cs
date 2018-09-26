using System.Reflection;
using Abp.EntityFramework;
using Abp.Modules;
using Abp.Reflection;
using Castle.Core.Logging;

namespace NBK.ECService.WMSLog.Repository
{
    [DependsOn(typeof(AbpEntityFrameworkModule))]
    public class WMSLogRepositoryModule: AbpModule
    {
        public ILogger Logger { get; set; }

        public WMSLogRepositoryModule(ITypeFinder typeFinder)
        {
            Logger = NullLogger.Instance;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}