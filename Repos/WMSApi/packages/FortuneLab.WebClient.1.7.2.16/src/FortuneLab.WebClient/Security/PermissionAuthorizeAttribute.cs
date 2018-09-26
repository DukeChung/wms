using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FortuneLab.WebClient.Security
{
    /// <summary>
    /// 权限授权验证
    /// </summary>
    public class PermissionAuthorizeAttribute : ActionFilterAttribute
    {
        public List<string> RequiredPermissionNames { get; private set; }

        public PermissionAuthorizeAttribute(params string[] requiredPermissions)
        {
            this.RequiredPermissionNames = new List<string>();
            this.RequiredPermissionNames.AddRange(requiredPermissions);
            this.Order = 1;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;

            var user = filterContext.HttpContext.User.Identity.Name;
            if (!AuthorizationService.Instance.FunctionAuthorize(RequiredPermissionNames.FirstOrDefault()))
            {
                filterContext.Result = UnAuthorizedResult();
            }
        }

        public ActionResult UnAuthorizedResult()
        {
            return new RedirectResult("/Account/AccessDenied");
        }
    }
}
