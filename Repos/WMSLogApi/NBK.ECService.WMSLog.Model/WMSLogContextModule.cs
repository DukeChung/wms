using System.Reflection;
using Abp.Modules;

namespace NBK.ECService.WMSLog.Model
{
    public class WMSLogContextModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}