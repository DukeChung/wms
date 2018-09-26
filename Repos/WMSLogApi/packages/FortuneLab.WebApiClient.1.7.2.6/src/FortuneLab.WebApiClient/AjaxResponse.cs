using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebApiClient
{
    internal class AjaxResponse<TResult>
    {
        //
        // 摘要:
        //     Error details (Must and only set if Abp.Web.Models.AjaxResponse`1.Success is
        //     false).
        public ErrorInfo Error { get; set; }
        //
        // 摘要:
        //     The actual result object of ajax request. It is set if Abp.Web.Models.AjaxResponse`1.Success
        //     is true.
        public TResult Result { get; set; }
        //
        // 摘要:
        //     Indicates success status of the result. Set Abp.Web.Models.AjaxResponse`1.Error
        //     if this value is false.
        public bool Success { get; set; }
        //
        // 摘要:
        //     This property can be used to indicate that the current user has no privilege
        //     to perform this request.
        public bool UnAuthorizedRequest { get; set; }
    }

    internal class AjaxResponse : AjaxResponse<object>
    {

    }

    internal class ErrorInfo
    {
        //
        // 摘要:
        //     Error code.
        public int Code { get; set; }
        //
        // 摘要:
        //     Error details.
        public string Details { get; set; }
        //
        // 摘要:
        //     Error message.
        public string Message { get; set; }
        //
        // 摘要:
        //     Validation errors if exists.
        public ValidationErrorInfo[] ValidationErrors { get; set; }
    }

    public class ValidationErrorInfo
    {        //
        // 摘要:
        //     Relate invalid members (fields/properties).
        public string[] Members { get; set; }
        //
        // 摘要:
        //     Validation error message.
        public string Message { get; set; }
    }
}
