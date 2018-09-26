using Abp.Web.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace FortuneLab.ECService.Securities.Filters
{
    public class AnonymousAttribute : BaseActionFilterAttribute
    {
        public const string KeyName = "Anonymous";
        public AnonymousAttribute()
        {
            Order = 1;
        }

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            filterContext.ActionArguments.Add("Anonymous", 1); // set up for AuthorizeAttribute so login will be ignored.
            base.OnActionExecuting(filterContext);
        }
    }
}
