using System.Reflection;
using Abp.Modules;

namespace NBK.ECService.WMSReport.ApiController
{
    /// <summary>
    /// 
    /// </summary>
    public class WMSReportApiControllerModule : AbpModule
    {
        /// <summary>
        /// 
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}