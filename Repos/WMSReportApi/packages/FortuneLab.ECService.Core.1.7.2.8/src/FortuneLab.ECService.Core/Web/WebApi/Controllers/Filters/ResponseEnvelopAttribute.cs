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
    /// �����ص����ݼ��ŷ�
    /// </summary>
    public class ResponseEnvelopAttribute : BaseActionFilterAttribute, ITransientDependency
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null)    //�����Exception������ֱ����������
                return;

            if (actionExecutedContext.Response == null)
                return;

            //�Ƿ�����ŷ�
            bool isIgnoreEnvelope = false;
            if (actionExecutedContext.Request.Headers.Contains("IgnoreEnvelope"))
            {
                var flagValue = actionExecutedContext.Request.Headers.GetValues("IgnoreEnvelope").SingleOrDefault();
                if (flagValue.Contains('"'))
                    flagValue = flagValue.Trim('"').Trim();

                if (!bool.TryParse(flagValue, out isIgnoreEnvelope))
                {
                    isIgnoreEnvelope = false;//Ĭ�ϲ�����
                }
            }

            //������Ҫ����Ϻ����õ�API��������IgnoreEnvelope header, �����Ϻ��ĵ��ö�������ŷ�
            //����������Ŀͻ��˷�������󣬻���IgnoreEnvelope header, ���Բ���������ŷ�
            //�����Ŀͻ��˿���ͨ�����ַ�ʽ����Api
            //a: ͨ��FortuneLabWebApiClient.Post, ��BaseQuery, ���趨IgnoreEnvelope = true;
            //b: ͨ��FortuneLabWebApiClient.CallPlatform, ��PlatformQuery, ���趨IgnoreEnvelope = false;
            actionExecutedContext.Response.Headers.Add("IsHaveEnvelope", (!isIgnoreEnvelope).ToString());

            if (isIgnoreEnvelope)
            {
                //������ԣ���ֱ�ӷ���,����������ŷ�
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
                actionExecutedContext.Response.Content = new StringContent(JsonConvert.SerializeObject(new AjaxResponse(false) { Result = "δ֪���" }));
            }
        }
    }
}