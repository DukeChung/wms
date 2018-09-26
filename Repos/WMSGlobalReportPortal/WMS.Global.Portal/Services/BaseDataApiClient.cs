using System;
using System.Collections.Generic;
using FortuneLab.Models;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.Utility;
using WMS.Global.Portal.Services;
using WMS.Global.Portal.Models;
using NBK.ECService.WMSReport.DTO.Base;

namespace WMS.Global.Portal.Services
{

    public class BaseDataApiClient
    {

        private static readonly BaseDataApiClient instance = new BaseDataApiClient();

        private BaseDataApiClient()
        {
        }

        public static BaseDataApiClient GetInstance()
        {
            return instance;
        }

        #region 登陆相关

        /// <summary>
        /// 验证用户登陆
        /// </summary>
        /// <param name="query"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public ApiResponse<CommonResponse> UserLoginCheck(ApplicationUser user)
        {
            return ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "/Account/UserLoginCheck", new CoreQuery(), user);
        }


        public ApiResponse UserLoginSuccess(ApplicationUser user)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Account/UserLoginSuccess", new CoreQuery(), user);
        }

        public ApiResponse UserLoginFail(ApplicationUser user)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Account/UserLoginFail", new CoreQuery(), user);
        }

        #endregion 

        #region 仓库
        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ApiResponse<List<WareHouseDto>> GetAllWarehouse()
        {
            return ApiClient.Post<List<WareHouseDto>>(PublicConst.WmsReportApiUrl, "/Global/GetAllWarehouse");
        }

        #endregion

        #region 菜单
        public ApiResponse<List<MenuDto>> GetSystemMenuList()
        {
            var query = new CoreQuery();
            return ApiClient.Get<List<MenuDto>>(PublicConst.WmsReportApiUrl, "Account/GetSystemMenuList", query);
        }
        #endregion

        #region 系统代码SysCode

        /// <summary>
        /// 新增系统代码明细
        /// UOM  计量单位类型
        /// ShelfLifeType 保质期类型
        /// LocUsage 货位用途
        /// </summary>
        /// <param name="sysCodeType"></param>
        /// <returns></returns>
        public ApiResponse<List<SelectItem>> SelectItemSysCode(string sysCodeType, bool isActive = true)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysCodeType, isActive };
            return ApiClient.Get<List<SelectItem>>(PublicConst.WmsReportApiUrl, "Global/GetSelectBySysCode", query);
        }

        #endregion
    }
}