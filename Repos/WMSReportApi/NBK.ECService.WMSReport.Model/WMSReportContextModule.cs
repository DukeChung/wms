using System.Reflection;
using Abp.Modules;

namespace NBK.ECService.WMSReport.Model
{
    public class WMSReportContextModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}