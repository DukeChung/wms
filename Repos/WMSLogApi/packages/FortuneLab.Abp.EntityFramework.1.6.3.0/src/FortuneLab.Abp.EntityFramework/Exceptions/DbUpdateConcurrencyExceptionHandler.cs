using Abp.Exceptions;
using Abp.Web.WebApi.Exceptions;
using System.Data.Entity.Infrastructure;

namespace FortuneLab.ECService.EntityFramework.Exceptions
{
    [HandleException(typeof(DbUpdateConcurrencyException))]
    public class DbUpdateConcurrencyExceptionHandler : ExceptionHandlerBase<DbUpdateConcurrencyException>
    {
        public DbUpdateConcurrencyExceptionHandler() : base(false)
        {

        }

        protected override void Handle()
        {
            AddError("Suggestion", "一般发生在update操作的地方， 如果要update的对象在db里不存在，EF会报这个错误");
        }

        protected override string ErrorCode
        {
            get { return "DbUpdateConcurrencyException"; }
        }
    }
}
