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
    public class DefaultExceptionHandler : ExceptionHandlerBase<Exception>
    {
        public DefaultExceptionHandler() : base(false)
        {

        }

        protected override void Handle()
        {
            StatusCode = HttpStatusCode.InternalServerError;
            AddError("ExceptionType", Exception.GetType().FullName);
            AddError("FullStackTrace", Exception.FullStackTrace());
        }

        protected override string ErrorCode
        {
            get { return "APISystemLevel_Error"; }
        }

        protected override string ErrorMessage
        {
            get { return Exception.FullMessage(); }
        }
    }
}
