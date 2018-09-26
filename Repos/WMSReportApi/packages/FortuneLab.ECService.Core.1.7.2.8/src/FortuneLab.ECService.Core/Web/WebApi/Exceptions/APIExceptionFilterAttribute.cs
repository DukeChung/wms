using Abp.Dependency;
using Abp.Events.Bus;
using Abp.Exceptions;
using Abp.Reflection;
using Abp.Web.Models;
using Abp.Web.WebApi.Controllers.Filters;
using FortuneLab;
using FortuneLab.ECService.Web.WebApi;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Filters;

namespace Abp.Web.WebApi.Exceptions
{
    /// <summary>
    /// API异常处理Filter
    /// </summary>
    public class APIExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly Dictionary<Type, Type> _exceptionToHandlerMapping = new Dictionary<Type, Type>();
        readonly ILogger _logger = LogManager.GetLogger("FortuneLab.Framework.APIExceptionFilterAttribute");
        public APIExceptionFilterAttribute()
        {
            var types = IocManager.Instance.Resolve<ITypeFinder>().Find(x => typeof(IApiExceptionHandler).IsAssignableFrom(x) && !x.IsAbstract);

            foreach (var handlerType in types)
            {
                var attribute = handlerType.GetCustomAttribute<HandleException>(true);
                if (attribute != null)
                    _exceptionToHandlerMapping.Add(attribute.ExceptionType, handlerType);
                else if (!(handlerType == typeof(DefaultExceptionHandler)))
                {
                    _logger.Warn("Type {0} is a ApiExceptionHandler but not specify the specific error type", handlerType.FullName);
                }
            }
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            if (!context.Request.Properties.ContainsKey(DataMonitorAttribute.StopwatchKey))
            {
                context.Request.Properties.Add(DataMonitorAttribute.StopwatchKey, Stopwatch.StartNew());
            }

            var stopwatch = context.Request.Properties[DataMonitorAttribute.StopwatchKey] as Stopwatch;

            var apiTypeAttribute = context.ActionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<ApiTypeAttribute>().SingleOrDefault();
            if (apiTypeAttribute != null && apiTypeAttribute.ApiType == ApiTypeEnum.平台 && !WebApiRequestHelper.IsRequestIgnoreEnvelope(context.ActionContext))
            {
                PlatformErrorHandler(context);
            }
            else
            {
                InternalErrorHandler(context);
            }

            if (context.Response != null && stopwatch != null)
            {
                context.Response.Headers.Add("Duration", stopwatch.ElapsedMilliseconds.ToString());
                stopwatch.Stop();
            }

            WebApiRequestLogging.LogRequestInfo(context, stopwatch);
        }

        private void PlatformErrorHandler(HttpActionExecutedContext context)
        {
            var eventBus = IocManager.Instance.Resolve<IEventBus>();

            var ajaxResponse = new AjaxResponse(
                    ErrorInfoBuilder.Instance.BuildForException(context.Exception),
                    context.Exception is Abp.Authorization.AbpAuthorizationException);

            context.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(ajaxResponse))
            };
        }

        private void InternalErrorHandler(HttpActionExecutedContext context)
        {
            IApiExceptionHandler exceptionHandler = null;
            var type = context.Exception.GetType();
            if (_exceptionToHandlerMapping.ContainsKey(type))
            {
                exceptionHandler = Activator.CreateInstance(_exceptionToHandlerMapping[type]) as IApiExceptionHandler;// IocManager.Instance.Resolve() as IApiExceptionHandler;
            }
            if (exceptionHandler == null)
                exceptionHandler = new DefaultExceptionHandler();
            context.Response = exceptionHandler.GetResponseMessage(context.Exception);
        }
    }
}
