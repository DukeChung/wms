using Abp.Exceptions;
using Abp.Web.WebApi.Exceptions;

namespace Abp.Web.ExceptionHandlers
{
    [HandleException(typeof(BusinessException))]
    public class  BusinessExceptionHandler: ExceptionHandlerBase<BusinessException>
    {
        public BusinessExceptionHandler() : base(true)
        {

        }

        protected override string ErrorCode
        {
            get { return this.Exception.ErrorCode; }
        }

        protected override string ErrorMessage
        {
            get { return this.Exception.Message; }
        }
    }
}
