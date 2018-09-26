using System.Reflection;
using Abp.EntityFramework;
using Abp.Modules;
using Abp.Reflection;
using Castle.Core.Logging;

namespace NBK.ECService.WMSReport.Repository
{
    [DependsOn(typeof(AbpEntityFrameworkModule))]
    public class WMSReportRepositoryModule : AbpModule
    {
        public ILogger Logger { get; set; }

        public WMSReportRepositoryModule(ITypeFinder typeFinder)
        {
            Logger = NullLogger.Instance;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}