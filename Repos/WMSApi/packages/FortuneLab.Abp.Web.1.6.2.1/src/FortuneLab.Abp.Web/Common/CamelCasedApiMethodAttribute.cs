using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace FortuneLab.Web
{
    public class CamelCasedApiMethodAttribute : ActionFilterAttribute
    {
        private static JsonMediaTypeFormatter _camelCasingFormatter = new JsonMediaTypeFormatter();

        static CamelCasedApiMethodAttribute()
        {
            _camelCasingFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        public override void OnActionExecuted(HttpActionExecutedContext httpActionExecutedContext)
        {
            var objectContent = httpActionExecutedContext.Response.Content as ObjectContent;
            if (objectContent != null)
            {
                if (objectContent.Formatter is JsonMediaTypeFormatter)
                {
                    httpActionExecutedContext.Response.Content = new ObjectContent(objectContent.ObjectType, objectContent.Value, _camelCasingFormatter);
                }
            }
        }
    }
}
