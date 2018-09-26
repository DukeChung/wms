using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Web.WebApi.Exceptions
{
    public abstract class ExceptionHandlerBase<T> : IApiExceptionHandler
        where T : Exception
    {
        private Dictionary<string, object> ErrorItems = new Dictionary<string, object>();
        protected HttpStatusCode StatusCode = HttpStatusCode.InternalServerError;
        protected T Exception { get; private set; }

        protected abstract string ErrorCode { get; }
        protected virtual string ErrorMessage { get; set; }
        protected bool IsBusinessException { get; private set; }
        public ExceptionHandlerBase(bool isBusinessException)
        {
            this.IsBusinessException = isBusinessException;
        }

        protected void AddError(string key, object value)
        {
            if (ErrorItems.ContainsKey(key))
                ErrorItems[key] = value;
            else
                ErrorItems.Add(key, value);
        }

        protected virtual void Handle() { }

        public virtual HttpResponseMessage GetResponseMessage(Exception exception)
        {
            this.Exception = exception as T;
            ErrorItems.Add("ErrorCode", ErrorCode);
            ErrorItems.Add("ErrorMessage", string.IsNullOrWhiteSpace(ErrorMessage) ? this.Exception.FullMessage() : ErrorMessage);
            ErrorItems.Add("IsBusinessException", IsBusinessException);
            Handle();

            return new HttpResponseMessage
            {
                StatusCode = StatusCode,
                Content = new ObjectContent<Dictionary<string, object>>(ErrorItems, new System.Net.Http.Formatting.JsonMediaTypeFormatter())
                //  new StringContent(JsonConvert.SerializeObject(ErrorItems))
            };
        }
    }
}
