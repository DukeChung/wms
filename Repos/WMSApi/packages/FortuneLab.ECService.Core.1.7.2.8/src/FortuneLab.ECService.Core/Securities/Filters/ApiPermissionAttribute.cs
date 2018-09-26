using Abp.Dependency;
using Abp.Web.WebApi;
using FortuneLab.ECService.Securities.Entities;
using FortuneLab.Infrastructure;
using FortuneLab.Runtime;
using System;
using System.Configuration;
using System.Web.Http.Controllers;

namespace FortuneLab.ECService.Securities.Filters
{
    public class ApiPermissionAttribute : BaseActionFilterAttribute
    {
        public string PermissionName { get; set; }

        public ApiPermissionAttribute(string permissionName)
        {
            Order = 3;
            this.PermissionName = permissionName;
        }

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            if (PermissionCheckConfig.Instance.PermissionCheckEnable && filterContext.ControllerContext.RouteData.Values.ContainsKey(TokenName))
            {
                ApiToken CurToken = filterContext.ControllerContext.RouteData.Values[TokenName] as ApiToken;
                SystemUser LogonUser = filterContext.ControllerContext.RouteData.Values[LogonUserName] as SystemUser;

                if (CurToken != null)
                {
                    //API Permission Authorization
                    var token = CurToken.Token;
                    var userID = LogonUser.LoginName;
                    var funAuthKey = PermissionName;
                    var _userService = IocManager.Instance.Resolve<IUserAppService>();
                    var isPassedAuth = _userService.CheckUserAuthorization(token, userID, funAuthKey);
                    if (!isPassedAuth)
                    {
                        throw new Exception(string.Format("API Auth failed!Token:{0},UserID:{1},FunAuthKey:{2}", token, userID, funAuthKey));
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
