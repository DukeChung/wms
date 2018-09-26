using FortuneLab;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Filters;
using System.Linq;

namespace Swagger.Net
{
    /// <summary>
    /// Determines if any request hit the Swagger route. Moves on if not, otherwise responds with JSON Swagger spec doc
    /// </summary>
    public class SwaggerActionFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Executes each request to give either a JSON Swagger spec doc or passes through the request
        /// </summary>
        /// <param name="actionContext">Context of the action</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var docRequest = actionContext.ControllerContext.RouteData.Values.ContainsKey(SwaggerGen.SWAGGER);

            if (!docRequest)
            {
                base.OnActionExecuting(actionContext);
                return;
            }

            HttpResponseMessage response = new HttpResponseMessage();

            response.Content = new ObjectContent<ResourceListing>(
                getDocs(actionContext),
                actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);

            actionContext.Response = response;
        }

        /// <summary>
        /// 获取子项API描述原数据
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private ResourceListing getDocs(HttpActionContext actionContext)
        {
            var assemblyType = (actionContext.ActionDescriptor as ReflectedHttpActionDescriptor).MethodInfo.DeclaringType;
            var docProvider = new XmlCommentDocumentationProvider(); //(XmlCommentDocumentationProvider)GlobalConfiguration.Configuration.Services.GetDocumentationProvider();

            ResourceListing r = SwaggerGen.CreateResourceListing(actionContext);

            var apis = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;

            foreach (var api in apis)
            {
                if (api.ActionDescriptor.ActionName.EndsWith("API"))//Ignore each Default API action
                    continue;

                string apiControllerName = api.ActionDescriptor.ControllerDescriptor.ControllerName;
                if (api.Route.Defaults.ContainsKey(SwaggerGen.SWAGGER) ||
                    apiControllerName.ToUpper().Equals(SwaggerGen.SWAGGER.ToUpper()))
                    continue;

                // Make sure we only report the current controller docs
                if (!apiControllerName.Equals(actionContext.ControllerContext.ControllerDescriptor.ControllerName))
                    continue;

                //如果是type=1, 只显示平台的API
                //int type = 1;

                //if (!int.TryParse((actionContext.ControllerContext.RouteData.Values["type"] ?? string.Empty).ToString(), out type))
                //{
                //    type = 1;
                //}

                //var isHaveResponseEnvelop = api.ActionDescriptor.GetFilters().SingleOrDefault(x => x.GetType().Equals(typeof(ResponseEnvelopAttribute))) != null;
                //if (type == 1 && !isHaveResponseEnvelop)
                //{
                //    continue;
                //}

                ResourceApi rApi = SwaggerGen.CreateResourceApi(api);
                r.apis.Add(rApi);

                ResourceApiOperation rApiOperation = SwaggerGen.CreateResourceApiOperation(r, api, docProvider);
                rApi.operations.Add(rApiOperation);

                foreach (var param in api.ParameterDescriptions)
                {
                    ResourceApiOperationParameter parameter = SwaggerGen.CreateResourceApiOperationParameter(r, api, param, docProvider);
                    rApiOperation.parameters.Add(parameter);
                }

                var isPlatformAPI = api.IsPlatformAPI();

                if (!isPlatformAPI && System.Configuration.ConfigurationManager.AppSettings["swagger:APITOKEN"] != null &&
                    System.Configuration.ConfigurationManager.AppSettings["swagger:APITOKEN"].Equals("true") &&
                    !api.ActionDescriptor.ActionName.EndsWith("API"))
                {
                    //var isPermissionNeeded = api.IsPermissionNeeded();
                    //if (isPermissionNeeded)
                    //{
                    //    rApiOperation.parameters.Insert(0, new ResourceApiOperationParameter() { name = "SessionKey", description = "Login SessionKey", required = true, paramType = "path", dataType = "String" });
                    //}

                    //添加Token
                    ResourceApiOperationParameter p = new ResourceApiOperationParameter();
                    p.name = "ApiToken";
                    p.description = "Api Token";
                    p.paramType = "path";
                    p.required = true;
                    p.dataType = "String";
                    rApiOperation.parameters.Insert(0, p);
                }

                //针对平台的API增加OperationUserId参数

                if (isPlatformAPI)
                {
                    ResourceApiOperationParameter p = new ResourceApiOperationParameter();
                    p.name = "OperationUserId";
                    p.description = "当前调用用户";
                    p.paramType = "path";
                    p.required = false;
                    p.dataType = "String";
                    rApiOperation.parameters.Insert(0, p);
                }

                SwaggerGen.CreateModel(r, api, docProvider);
                //r.models = new ResourceApiModel();
            }

            return r;
        }
    }

    public static class ApiDescriptionExtesion
    {
        public static bool IsPlatformAPI(this ApiDescription api)
        {
            var apiTypeAttribute = api.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<ApiTypeAttribute>().SingleOrDefault();
            return apiTypeAttribute != null && apiTypeAttribute.ApiType == ApiTypeEnum.平台;
        }

        //public static bool IsPermissionNeeded(this ApiDescription api)
        //{
        //    var apiTypeAttribute = api.ActionDescriptor.GetFilters().SingleOrDefault(x => typeof(ApiPermissionAttribute).IsAssignableFrom(x.GetType()));
        //    return apiTypeAttribute != null;
        //    return true;
        //}
    }
}
