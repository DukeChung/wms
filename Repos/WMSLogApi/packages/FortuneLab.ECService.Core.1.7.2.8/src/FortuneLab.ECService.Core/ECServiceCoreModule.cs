using Abp.Modules;
using Abp.Web.WebApi;
using Abp.Web.WebApi.Controllers.Filters;
using Abp.Web.WebApi.Exceptions;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Linq;
using FortuneLab.ECService.Auditing;
using Abp.Web.Models;
using Abp.Exceptions;
using System;
using FortuneLab.Logging.TargetExtension;
using NLog;

namespace FortuneLab.ECService
{
    //[DependsOn(typeof(CoreModule), typeof(LandRoverDomainModule))]
    public class ECServiceCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Configuration.DefaultNameOrConnectionString = "Default";
            LogManager.Configuration.ConfigRabbitMQTarget();//配置RabbitMQ日志
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            RegisterBusinessExceptionErrorInfoConverter();

            RegisterWebApiConfiguration();
        }

        private void RegisterBusinessExceptionErrorInfoConverter()
        {
            ErrorInfoBuilder.Instance.AddExceptionConverter(new BusinessExceptionErrorInfoConverter());
        }

        private static void RegisterWebApiConfiguration()
        {
            GlobalConfiguration.Configuration.MapHttpAttributeRoutes();
            GlobalConfiguration.Configuration.Filters.Add(new DataMonitorAttribute());
            GlobalConfiguration.Configuration.Filters.Add(new APIExceptionFilterAttribute());

            GlobalConfiguration.Configuration.Services.Add(typeof(System.Web.Http.Filters.IFilterProvider), new CustomFilterProvider());
            var providers = GlobalConfiguration.Configuration.Services.GetFilterProviders();
            var defaultprovider = providers.First(i => i is ActionDescriptorFilterProvider);

            GlobalConfiguration.Configuration.Services.Remove(typeof(System.Web.Http.Filters.IFilterProvider), defaultprovider);
        }
    }

    public class BusinessExceptionErrorInfoConverter : IExceptionToErrorInfoConverter
    {

        public ErrorInfo Convert(Exception exception)
        {
            ErrorInfo erroInfo = null;
            if (Next != null)
                erroInfo = Next.Convert(exception);

            if (erroInfo == null)
                erroInfo = new ErrorInfo();

            if (exception is BusinessException)
            {
                var bizException = (exception as BusinessException);
                int errorCode = 0;
                if (int.TryParse("6" + bizException.ErrorCode, out errorCode))
                    erroInfo.Code = errorCode;

                erroInfo.Message = bizException.Message;
            }
            return erroInfo;
        }

        public IExceptionToErrorInfoConverter Next { get; set; }
    }
}
