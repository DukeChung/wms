using Abp.Web.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace FortuneLab.ECService.Securities.Filters
{
    public class PublicAttribute : BaseActionFilterAttribute
    {
        public const string KeyName = "Public";
        public PublicAttribute()
        {
            Order = 1;
        }

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            if (!filterContext.ActionArguments.ContainsKey(KeyName))
                filterContext.ActionArguments.Add(KeyName, 1); // set up for AuthorizeAttribute so login will be ignored.
            base.OnActionExecuting(filterContext);
        }
    }

    public class BindDeafultValueAttribute : BaseActionFilterAttribute
    {
        public BindDeafultValueAttribute()
        {
            Order = 999;
        }

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            //if (!filterContext.ActionArguments.ContainsKey(KeyName))
            //    filterContext.ActionArguments.Add(KeyName, 1); // set up for AuthorizeAttribute so login will be ignored.
            //base.OnActionExecuting(filterContext);
            //foreach (var item in filterContext.ActionArguments)
            //{
            //    if (item.Value != null)
            //        continue;
            //    var proerpty = filterContext.ActionDescriptor.ActionBinding.ParameterBindings.SingleOrDefault(x => x.Descriptor.ParameterName == item.Key);

            //    if (proerpty != null && proerpty.Descriptor.DefaultValue != null)
            //        item.Value = proerpty.Descriptor.DefaultValue;
            //}
            //filterContext.ActionArguments
            //            filterContext.ActionDescriptor.ActionBinding.ParameterBindings
        }
    }
}
