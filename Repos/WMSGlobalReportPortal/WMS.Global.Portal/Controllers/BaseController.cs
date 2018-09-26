using FortuneLab.WebApiClient.Query;
using NBK.AuthServiceUtil;
using NBK.ECService.WMSReport.DTO;
using WMS.Global.Portal.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using NBK.ECService.WMSReport.DTO.Base;
using WMS.Global.Portal.Services;

namespace WMS.Global.Portal.Controllers
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

        public List<WareHouseDto> AllWareHouse
        {
            get
            {
                var rsp = BaseDataApiClient.GetInstance().GetAllWarehouse();
                if (rsp.Success && rsp.ResponseResult != null && rsp.ResponseResult.Count > 0)
                {
                    WareHouseDto all = new WareHouseDto { SysId = Guid.Empty, Name = "全部仓库" };
                    rsp.ResponseResult.Insert(0, all);
                    return rsp.ResponseResult;
                }
                else
                {
                    return new List<WareHouseDto>();
                }
            }
        }

        public static List<MenuDto> GetSystemMenu(string userName)
        {
            var response = BaseDataApiClient.GetInstance().GetSystemMenuList();
            List<MenuDto> menuList = new List<MenuDto>();
            if (response.Success)
            {
                menuList = response.ResponseResult;
                var authList = AuthorizeManager.GetUserAuthKey(userName);

                for (int i = (menuList.Count - 1); i >= 0; i--)
                {
                    //判断没有菜单权限，删除当前菜单显示
                    if (!string.IsNullOrEmpty(menuList[i].AuthKey)
                        && authList.FirstOrDefault(p => p.Key.Equals(menuList[i].AuthKey,
                            StringComparison.OrdinalIgnoreCase)) == null)
                    {
                        menuList.RemoveAt(i);
                    }
                }
                //移除没有二级菜单的一级菜单
                var firstLevelMenu = menuList.Where(p => p.ParentSysId == null).ToList();
                for (int i = 0; i < firstLevelMenu.Count(); i++)
                {
                    if (!menuList.Exists(p => p.ParentSysId == firstLevelMenu[i].SysId))
                    {
                        menuList.RemoveAll(p => p.SysId == firstLevelMenu[i].SysId);
                    }
                }
            }
            return menuList;
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
        }
    }
}