using Abp.Application.Services;
using NBK.ECService.WMSReport.Application.Interface;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.Model.Models;
using NBK.ECService.WMSReport.Repository.Interface;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.Application
{
    public class AccountAppService : ApplicationService, IAccountAppService
    {
        private ICrudRepository _crudRepository = null;
        private IBaseRepository _baseRepository = null;

        public AccountAppService(IBaseRepository baseRepository, ICrudRepository crudRepository)
        {
            this._baseRepository = baseRepository;
            this._crudRepository = crudRepository;
        }


        public CommonResponse UserLoginCheck(LoginUserInfo user)
        {
            CommonResponse response = new CommonResponse();

            var loginUserList = RedisWMS.GetRedisList<List<LoginUserInfo>>(RedisReportSourceKey.RedisReportLoginUserList);
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

        /// <summary>
        /// 获取系统菜单
        /// </summary>
        /// <returns></returns>
        public List<MenuDto> GetSystemMenuList()
        {
            _crudRepository.ChangeGlobalDB();
            return RedisWMS.CacheResult(() =>
            {
                return _baseRepository.GetSystemMenuList();
            }, RedisReportSourceKey.RedisReportMenuList);
        }

        /// <summary>
        /// 重新同步菜单
        /// </summary>
        public void SynchroMenu()
        {
            _crudRepository.ChangeGlobalDB();
            RedisWMS.CleanRedis<menu>(RedisReportSourceKey.RedisReportMenuList);
            var menuList = _baseRepository.GetSystemMenuList();
            RedisWMS.SetRedis(menuList, RedisReportSourceKey.RedisReportMenuList);
        }
    }
}
