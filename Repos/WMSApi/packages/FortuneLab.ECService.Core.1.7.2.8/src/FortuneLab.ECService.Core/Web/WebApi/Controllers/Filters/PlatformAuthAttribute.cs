using Abp.Securities;
using FortuneLab.ECService.Securities.Filters;
using FortuneLab.Exceptions;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace Abp.Web.WebApi.Controllers.Filters
{
    public class PlatformAuthAttribute : SessionValidateAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (WebApiRequestHelper.IsRequestIgnoreEnvelope(actionContext))
            {
                //如果是西安的客户端调用,则直接走西安的授权机制
                base.OnActionExecuting(actionContext);
                return;
            }

            //走专门给上海的授权机制
            string operationUserId = null;
            if (actionContext.Request.Headers.Contains("OperationUserId"))
            {
                operationUserId = actionContext.Request.Headers.GetValues("OperationUserId").SingleOrDefault();
            }
            else
            {
                var queryStrings = actionContext.Request.RequestUri.ParseQueryString();
                operationUserId = queryStrings["OperationUserId"];
            }

            int userId = 0;
            if (!int.TryParse(operationUserId, out userId))
            {
                userId = 0;
            }

            if (userId == 0)
            {
                SetRequestProperty(actionContext, new FortuneLabUserPrincipal<int>());
            }
            else
            {
                SetRequestProperty(actionContext, new FortuneLabUserPrincipal<int>(new PlatformUser(userId), "PLATFORM"));
            }
        }
    }

    public class WebApiRequestHelper
    {
        /// <summary>
        /// 检查客户端请求是否忽略信封, 如果忽略代表是西安的客户端，没有则是上海的客户端
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        /// <returns></returns>
        public static bool IsRequestIgnoreEnvelope(HttpActionContext actionExecutedContext)
        {
            //是否忽略信封
            bool isIgnoreEnvelope = false;
            if (actionExecutedContext.Request.Headers.Contains("IgnoreEnvelope"))
            {
                if (!bool.TryParse(actionExecutedContext.Request.Headers.GetValues("IgnoreEnvelope").Single(), out isIgnoreEnvelope))
                {
                    isIgnoreEnvelope = false;//默认不忽略
                }
            }
            return isIgnoreEnvelope;
        }
    }
}
