using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Web.WebApi.Exceptions
{
    public interface IApiExceptionHandler
    {
        HttpResponseMessage GetResponseMessage(Exception exception);
    }
}
