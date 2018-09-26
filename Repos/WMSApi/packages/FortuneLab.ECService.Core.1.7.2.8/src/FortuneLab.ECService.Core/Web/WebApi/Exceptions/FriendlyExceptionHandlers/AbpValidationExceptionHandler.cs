using Abp.Exceptions;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Web.WebApi.Exceptions
{
    [HandleException(typeof(AbpValidationException))]
    public class AbpValidationExceptionHandler : ExceptionHandlerBase<AbpValidationException>
    {
        public AbpValidationExceptionHandler() : base(true)
        {

        }

        protected override string ErrorCode
        {
            get { return "ValidationError"; }
        }

        protected override string ErrorMessage
        {
            get { return string.Join(",", Exception.ValidationErrors.Select(x => x.ErrorMessage)); }
        }
    }
}
