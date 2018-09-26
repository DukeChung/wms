using FortuneLab.WebApiClient.Query;
using NBK.AuthServiceUtil;
using NBK.ECService.WMS.DTO;
using NBK.WMS.Portal.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMS.Portal.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {

        }

        private ApplicationUser currentUser;

        public ApplicationUser CurrentUser
        {
            get
            {
                var identity = HttpContext.User.Identity;
                var claimsIdentity = identity as ClaimsIdentity;
                if (claimsIdentity == null) return null;

                var claims = claimsIdentity.Claims.ToDictionary(x => x.Type, x => x.Value);
                currentUser = claims.ContainsKey(SystemDataConst.LoginUser) ? JsonConvert.DeserializeObject<ApplicationUser>(claims[SystemDataConst.LoginUser]) : null;

                return currentUser;
            }
        }

        public CoreQuery LoginCoreQuery
        {
            get
            {
                var query = new CoreQuery();
                if (CurrentUser != null)
                    query.ParmsObj = new { SessionKey = CurrentUser.UserId };
                return query;
            }
        }

        public bool CheckHasAuthFun(string authKeyName)
        {
            AuthKey authKey = new AuthKey(authKeyName);
            return AuthorizeManager.HasFunction(authKey, CurrentUser.UserName);
        }

        public bool CheckHasAuthFun(AuthKey authKey)
        {
            return AuthorizeManager.HasFunction(authKey, CurrentUser.UserName);
        }
    }

    /// <summary>
    /// 赋值用户登录信息标签，action 接收到 baseDto时自动对相关userid等信息赋值
    /// </summary>
    public class SetUserInfoAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var baseDtoValue = filterContext.ActionParameters.FirstOrDefault(p => p.Value is BaseDto).Value;
            var baseController = filterContext.Controller as BaseController;
            if (baseDtoValue != null && baseController != null)
            {
                var baseDto = baseDtoValue as BaseDto;
                baseDto.CurrentUserId = baseController.CurrentUser.UserId;
                baseDto.CurrentDisplayName = baseController.CurrentUser.DisplayName;
                baseDto.WarehouseSysId = baseController.CurrentUser.WarehouseSysId;
            }

            //var baseQueryValue = filterContext.ActionParameters.FirstOrDefault(p => p.Value is BaseQuery).Value;
            //if (baseQueryValue != null && baseController != null)
            //{
            //    var baseQuery = baseQueryValue as BaseQuery;
            //    baseQuery.WarehouseSysId = baseController.CurrentUser.WarehouseSysId;
            //}
        }
    }
}