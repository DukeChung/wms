using Abp.Dependency;
using Abp.Web.Models;
using Abp.Web.WebApi;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using System.Linq;

namespace Abp.WebApi.Controllers.Filters
{
    /// <summary>
    /// 给返回的数据加信封
    /// </summary>
    public class ResponseEnvelopAttribute : BaseActionFilterAttribute, ITransientDependency
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null)    //如果有Exception发生，直接跳过处理
                return;

            if (actionExecutedContext.Response == null)
                return;

            //是否忽略信封
            bool isIgnoreEnvelope = false;
            if (actionExecutedContext.Request.Headers.Contains("IgnoreEnvelope"))
            {
                var flagValue = actionExecutedContext.Request.Headers.GetValues("IgnoreEnvelope").SingleOrDefault();
                if (flagValue.Contains('"'))
                    flagValue = flagValue.Trim('"').Trim();

                if (!bool.TryParse(flagValue, out isIgnoreEnvelope))
                {
                    isIgnoreEnvelope = false;//默认不忽略
                }
            }

            //这里主要针对上海调用的API，不会有IgnoreEnvelope header, 所有上海的调用都会加上信封
            //如果是西安的客户端发起的请求，会有IgnoreEnvelope header, 所以并不会加上信封
            //西安的客户端可以通过两种方式调用Api
            //a: 通过FortuneLabWebApiClient.Post, 传BaseQuery, 会设定IgnoreEnvelope = true;
            //b: 通过FortuneLabWebApiClient.CallPlatform, 传PlatformQuery, 会设定IgnoreEnvelope = false;
            actionExecutedContext.Response.Headers.Add("IsHaveEnvelope", (!isIgnoreEnvelope).ToString());

            if (isIgnoreEnvelope)
            {
                //如果忽略，就直接返回,否则继续加信封
                return;
            }

            if (actionExecutedContext.Response.Content is ObjectContent)
            {
                var content = actionExecutedContext.Response.Content as ObjectContent;
                actionExecutedContext.Response.Content = new StringContent(JsonConvert.SerializeObject(new AjaxResponse(content.Value)));
            }
            else if (HttpContext.Current.Response.OutputStream.CanRead)
            {
                var content = new StreamReader(HttpContext.Current.Response.OutputStream).ReadToEnd();
                actionExecutedContext.Response.Content = new StringContent(JsonConvert.SerializeObject(new AjaxResponse(content)));
            }
            else if (actionExecutedContext.ActionContext.ActionDescriptor.ReturnType == null)
            {
                actionExecutedContext.Response.StatusCode = HttpStatusCode.OK;
                actionExecutedContext.Response.Content = new StringContent(JsonConvert.SerializeObject(new AjaxResponse(true)));
            }
            else
            {
                actionExecutedContext.Response.StatusCode = HttpStatusCode.OK;
                actionExecutedContext.Response.Content = new StringContent(JsonConvert.SerializeObject(new AjaxResponse(false) { Result = "未知结果" }));
            }
        }
    }
}