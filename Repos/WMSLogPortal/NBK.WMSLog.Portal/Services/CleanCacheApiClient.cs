using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace NBK.WMSLog.Portal.Services
{
    public class CleanCacheApiClient
    {
        private static readonly CleanCacheApiClient instance = new CleanCacheApiClient();

        private CleanCacheApiClient() { }

        public static CleanCacheApiClient GetInstance() { return instance; }


        /// <summary>
        /// 同步SKU包含分类全部
        /// </summary>
        /// <returns></returns>
        public ApiResponse SynchroAll()
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Redis/SynchroAll");
        }

        /// <summary>
        /// 同步菜单
        /// </summary>
        /// <returns></returns>
        public ApiResponse SynchroMenu()
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Redis/SynchroMenu");
        }

        /// <summary>
        /// 同步登陆信息
        /// </summary>
        /// <returns></returns>
        public ApiResponse CleanUserLoginRedis()
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Redis/CleanUserLoginRedis");
        }

        /// <summary>
        /// 同步SKU包含分类
        /// </summary>
        /// <returns></returns>
        public ApiResponse SynchroSku()
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Redis/SynchroSku");
        }

        /// <summary>
        /// 同步包装包含单位
        /// </summary>
        /// <returns></returns>
        public ApiResponse SynchroPack()
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Redis/SynchroPack");
        }

        /// <summary>
        /// 同步批次模板
        /// </summary>
        /// <returns></returns>
        public ApiResponse SynchroLottemplate()
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Redis/SynchroLottemplate");
        }

        /// <summary>
        /// 同步仓库
        /// </summary>
        /// <returns></returns>
        public ApiResponse SynchroWarehouse()
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Redis/SynchroWarehouse");
        }

        /// <summary>
        /// 根据key值获取同步前Redis信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        
        public ApiResponse<string> GetRedis(CoreQuery query, string key)
        {
            query.ParmsObj = new { key = key };
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "/Redis/GetRedis",query);
        } 
    }
}