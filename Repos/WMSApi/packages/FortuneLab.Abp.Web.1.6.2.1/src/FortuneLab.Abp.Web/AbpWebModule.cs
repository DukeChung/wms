using Abp.Dependency;
using Abp.Modules;
using Abp.WebApi.Configuration;
using Abp.WebApi.Controllers;
using Abp.WebApi.Controllers.Dynamic;
using Abp.WebApi.Controllers.Dynamic.Selectors;
using Castle.MicroKernel.Registration;
using Newtonsoft.Json.Serialization;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Filters;

namespace Abp.Web
{
    public class FortuneLabAbpWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new ApiControllerConventionalRegistrar());
            IocManager.Register<IAbpWebApiModuleConfiguration, AbpWebApiModuleConfiguration>();
        }

        public override void Initialize()
        {
            var httpConfiguration = IocManager.Resolve<IAbpWebApiModuleConfiguration>().HttpConfiguration;

            InitializeAspNetServices(httpConfiguration);
            InitializeFilters(httpConfiguration);
            InitializeFormatters(httpConfiguration);
            InitializeRoutes(httpConfiguration);
        }

        private void InitializeAspNetServices(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.Services.Replace(typeof(IHttpControllerSelector), new AbpHttpControllerSelector(httpConfiguration));
            httpConfiguration.Services.Replace(typeof(IHttpActionSelector), new AbpApiControllerActionSelector());
            httpConfiguration.Services.Replace(typeof(IHttpControllerActivator), new AbpApiControllerActivator(IocManager));
        }

        private void InitializeFilters(HttpConfiguration httpConfiguration)
        {
            //httpConfiguration.Filters.Add(IocManager.Resolve<AbpExceptionFilterAttribute>());
        }

        private void InitializeFormatters(HttpConfiguration httpConfiguration)
        {
            //httpConfiguration.Formatters.Clear();
            //var formatter = new JsonMediaTypeFormatter();
            //formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //httpConfiguration.Formatters.Add(formatter);
            //httpConfiguration.Formatters.Add(new PlainTextFormatter());

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            //config.EnableSystemDiagnosticsTracing();

            //http://tostring.it/2012/07/18/customize-json-result-in-web-api/
            var json = httpConfiguration.Formatters.JsonFormatter;
            //ISO (default): "ReleaseDate": "1901-01-08T04:00:00",
            //MicrosoftDateFormat:"ReleaseDate": "/Date(-2176862400000+0800)/",
            json.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
            //all datetime translate to GMT/UTC
            json.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
            json.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            json.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            //json.SerializerSettings.TypeNameHandling= Newtonsoft.Json.TypeN

            if ("true".Equals(ConfigurationManager.AppSettings["fortunelab:camelJsonPropertyName"], System.StringComparison.OrdinalIgnoreCase))
            {
                json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            HttpConfiguration config = httpConfiguration;
            //to remove xml seralization
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml"));
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "text/xml"));

        }

        /*
          var jsonMediaTypeFormatter = httpControllerSettings.Formatters.OfType<JsonMediaTypeFormatter>().Single();
        httpControllerSettings.Formatters.Remove(jsonMediaTypeFormatter);

        jsonMediaTypeFormatter = new JsonMediaTypeFormatter
        {
            SerializerSettings =
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }
        };

        httpControllerSettings.Formatters.Add(jsonMediaTypeFormatter);
         */

        private void InitializeRoutes(HttpConfiguration httpConfiguration)
        {
            //DynamicApiRouteConfig.Register(httpConfiguration);
        }
    }
}
