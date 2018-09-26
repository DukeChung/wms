using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Account;
using NBK.ECService.WMS.Utility.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application
{
    public class AccountAppService : WMSApplicationService, IAccountAppService
    {
        public CommonResponse UserLoginCheck(LoginUserInfo user)
        {
            CommonResponse response = new CommonResponse();

            var loginUserList = RedisWMS.GetRedisList<List<LoginUserInfo>>(RedisSourceKey.RedisLoginUserList);
            if (loginUserList != null)
            {
                var loginUser = loginUserList.Where(p => p.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (loginUser != null)
                {
                    if (loginUser.RetryLoginCount >= 3)
                    {
                        TimeSpan sp = DateTime.Now - loginUser.LoginDate;

                        if (sp.TotalMinutes < 10)
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "失败登陆次数已超过3次，请稍后再试!";
                        }
                    }
                }
            }

            return response;
        }

        public void UserLoginSuccess(LoginUserInfo user)
        {
            user.LoginDate = DateTime.Now;
            var loginUserList = RedisWMS.GetRedisList<List<LoginUserInfo>>(RedisSourceKey.RedisLoginUserList);
            if (loginUserList != null)
            {
                var loginUser = loginUserList.Where(p => p.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (loginUser != null)
                {
                    loginUser.RetryLoginCount = 0;
                    loginUser.LoginDate = DateTime.Now;
                }
                else
                {
                    user.RetryLoginCount = 0;
                    loginUserList.Add(user);
                }
            }
            else
            {
                loginUserList = new List<LoginUserInfo>();
                user.RetryLoginCount = 0;
                loginUserList.Add(user);
            }

            RedisWMS.SetRedis(loginUserList, RedisSourceKey.RedisLoginUserList);
        }

        public void UserLoginFail(LoginUserInfo user)
        {
            user.LoginDate = DateTime.Now;
            var loginUserList = RedisWMS.GetRedisList<List<LoginUserInfo>>(RedisSourceKey.RedisLoginUserList);
            if (loginUserList != null)
            {
                var loginUser = loginUserList.Where(p => p.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (loginUser != null)
                {
                    loginUser.RetryLoginCount = loginUser.RetryLoginCount + 1;
                    loginUser.LoginDate = DateTime.Now;
                }
                else
                {
                    user.RetryLoginCount = 1;
                    loginUserList.Add(user);
                }
            }
            else
            {
                loginUserList = new List<LoginUserInfo>();
                user.RetryLoginCount = 1;
                loginUserList.Add(user);
            }

            RedisWMS.SetRedis(loginUserList, RedisSourceKey.RedisLoginUserList);
        }
    }
}
