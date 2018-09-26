using FortuneLab;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Swagger.Net
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SwaggerController : ApiController
    {
        /// <summary>
        /// 获取API大目录结构
        /// Get the resource description of the api for swagger documentation
        /// </summary>
        /// <remarks>It is very convenient to have this information available for generating clients. This is the entry point for the swagger UI
        /// </remarks>
        /// <returns>JSON document representing structure of API</returns>
        public HttpResponseMessage Get(int type = 1)
        {
            var docProvider = (XmlCommentDocumentationProvider)GlobalConfiguration.Configuration.Services.GetDocumentationProvider();

            ResourceListing r = SwaggerGen.CreateResourceListing(ControllerContext);
            List<string> uniqueControllers = new List<string>();

            var apis = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;

            foreach (var api in apis)
            {
                string controllerName = api.ActionDescriptor.ControllerDescriptor.ControllerName;
                if (uniqueControllers.Contains(controllerName) ||
                      controllerName.ToUpper().Equals(SwaggerGen.SWAGGER.ToUpper())
                    || controllerName.ToLower().StartsWith("abp")
                    ) continue;

                uniqueControllers.Add(controllerName);

                //针对平台API跟我们内部API加不同的展示
                var apiTypeAttribute = api.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<ApiTypeAttribute>().SingleOrDefault();

                if (type == 1)
                {
                    if (apiTypeAttribute != null && (int)apiTypeAttribute.ApiType == type)
                    {
                        ResourceApi rApi = SwaggerGen.CreateResourceApi(api);
                        r.apis.Add(rApi);
                    }
                }
                else
                {
                    if (apiTypeAttribute == null || apiTypeAttribute.ApiType != ApiTypeEnum.平台)
                    {
                        ResourceApi rApi = SwaggerGen.CreateResourceApi(api);
                        r.apis.Add(rApi);
                    }
                }
            }

            HttpResponseMessage resp = new HttpResponseMessage();

            resp.Content = new ObjectContent<ResourceListing>(r, ControllerContext.Configuration.Formatters.JsonFormatter);

            return resp;
        }
    }
}
