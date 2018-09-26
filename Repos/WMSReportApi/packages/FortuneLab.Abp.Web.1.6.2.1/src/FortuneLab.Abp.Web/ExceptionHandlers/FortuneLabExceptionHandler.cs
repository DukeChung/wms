using Abp.Exceptions;
using Abp.Web.WebApi.Exceptions;
using FortuneLab.Exceptions;

namespace Abp.Web.ExceptionHandlers
{
    [HandleException(typeof(FortuneLabException))]
    public class FortuneLabExceptionHandler : ExceptionHandlerBase<FortuneLabException>
    {
        public FortuneLabExceptionHandler() : base(true)
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
