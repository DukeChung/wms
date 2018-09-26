using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMS.ApiController.BaseDataControllers
{
    [RoutePrefix("api/Account")]
    [AccessLog]
    public class AccountController : AbpApiController
    {
        private IAccountAppService _accountAppService;
        public AccountController(IAccountAppService accountAppService)
        {
            _accountAppService = accountAppService;
        }

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
        /// 登陆成功时调用
        /// </summary>
        /// <param name="user"></param>
        [HttpPost, Route("UserLoginSuccess")]
        public void UserLoginSuccess(LoginUserInfo user)
        {
            _accountAppService.UserLoginSuccess(user);
        }

        /// <summary>
        /// 登陆失败时调用
        /// </summary>
        /// <param name="user"></param>
        [HttpPost, Route("UserLoginFail")]
        public void UserLoginFail(LoginUserInfo user)
        {
            _accountAppService.UserLoginFail(user);
        }
    }
}
