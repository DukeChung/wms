using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.Modules;
using Abp.Web.Configuration;

namespace NBK.ECService.WMSLog.WebApi
{
    public class ECServiceWebApiModule : AbpModule
    {
        public override void PreInitialize()
        {
            //是否开启多租户
            IocManager.Resolve<IMultiTenancyConfig>().IsEnabled = true;

            //是否开启审计日志
            Configuration.Auditing.IsEnabled = true;
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            //LYM 输出所有错误信息到客户端，调试时用
            IocManager.Resolve<IAbpWebModuleConfiguration>().SendAllExceptionsToClients = true;


            Configuration.Localization.Languages.Add(new LanguageInfo("zh-CHS", "简体中文", "famfamfam-flag-cn", true));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}