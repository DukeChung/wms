using Abp.WebApi.Controllers;
using NBK.ECService.WMSReport.Application.Interface;
using NBK.ECService.WMSReport.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMSReport.ApiController
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/Account")]
    public class AccountController : AbpApiController
    {
        private IAccountAppService _accountAppService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountAppService"></param>
        public AccountController(IAccountAppService accountAppService)
        {
            _accountAppService = accountAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void AccountApi() { }

        /// <summary>
        /// 校验用户登陆验证
        /// #1. 失败登陆次数
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost, Route("UserLoginCheck")]
        public CommonResponse UserLoginCheck(LoginUserInfo user)
        {
            return _accountAppService.UserLoginCheck(user);
        }

        /// <summary>
        /// 获取系统菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetSystemMenuList")]
        public List<MenuDto> GetSystemMenuList()
        {
            return _accountAppService.GetSystemMenuList();
        }

        /// <summary>
        /// 同步菜单
        /// </summary>
        [HttpPost, Route("SynchroMenu")]
        public void SynchroMenu()
        {
            _accountAppService.SynchroMenu();
        }
    }
}
