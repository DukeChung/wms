using Abp.Exceptions;
using Abp.Web.WebApi.Exceptions;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace FortuneLab.ECService.EntityFramework.Exceptions
{
    [HandleException(typeof(DbEntityValidationException))]
    public class DbEntityValidationExceptionHandler : ExceptionHandlerBase<DbEntityValidationException>
    {
        public DbEntityValidationExceptionHandler() : base(false)
        {

        }

        protected override void Handle()
        {
            Dictionary<string, string> errorMessages = new Dictionary<string, string>();
            foreach (var item in this.Exception.EntityValidationErrors.SelectMany(x => x.ValidationErrors))
            {
                errorMessages.Add(item.PropertyName, item.ErrorMessage);
            }
            AddError("ValidationMessage", errorMessages);
        }

        protected override string ErrorCode
        {
            get { return "DbEntityValidationError"; }
        }
    }
}
