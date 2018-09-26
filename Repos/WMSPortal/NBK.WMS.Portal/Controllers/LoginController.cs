using NBK.AuthServiceUtil;
using NBK.ECService.WMS.DTO.System;
using NBK.WMS.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMS.Portal.Controllers
{
    public class LoginController : BaseController
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
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
}