using System.Web.Http;
using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.ThirdParty;
using System;
using NBK.ECService.WMS.Utility.Redis;

namespace NBK.ECService.WMS.ApiController.OtherControllers
{
    [RoutePrefix("api/Redis")]
    public class RedisController : AbpApiController
    {
        private IRedisAppService _redisAppService;
     
        /// <summary>
        /// 系统构造
        /// </summary>
        /// <param name="redisAppService"></param>
        /// <param name="chartAppService"></param>
        public RedisController(IRedisAppService redisAppService)
        {
            _redisAppService = redisAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void RedisAPI() { }

        /// <summary>
        /// 同步SKU  包含分类 
        /// </summary>
        [HttpPost, Route("SynchroAll")]
        public void SynchroAll()
        {
            _redisAppService.SynchroSku();
            _redisAppService.SynchroPack();
            _redisAppService.SynchroLottemplate();
            _redisAppService.SynchroWarehouse();
        }

        /// <summary>
        /// 同步菜单
        /// </summary>
        [HttpPost, Route("SynchroMenu")]
        public void SynchroMenu()
        {
            _redisAppService.SynchroMenu();
        } 


        /// <summary>
        /// 同步菜单
        /// </summary>
        [HttpPost, Route("CleanUserLoginRedis")]
        public void CleanUserLoginRedis()
        {
            _redisAppService.CleanUserLoginRedis();
        }



        /// <summary>
        /// 同步SKU  包含分类 
        /// </summary>
        [HttpPost, Route("SynchroSku")]
        public void SynchroSku()
        {
            _redisAppService.SynchroSku();
        }

        /// <summary>
        /// 同步包装 包含单位
        /// </summary>
        [HttpPost, Route("SynchroPack")]
        public void SynchroPack()
        {
            _redisAppService.SynchroPack();
        }

        /// <summary>
        /// 同步批次模板
        /// </summary>
        [HttpPost, Route("SynchroLottemplate")]
        public void SynchroLottemplate()
        {
            _redisAppService.SynchroLottemplate();
        }

        /// <summary>
        /// 同步仓库 
        /// </summary>
        [HttpPost, Route("SynchroWarehouse")]
        public void SynchroWarehouse()
        {
            _redisAppService.SynchroWarehouse();
        }



        /// <summary>
        /// 根据key获取Redis
        /// </summary>
        [HttpPost, Route("GetRedis")]
        public string GetRedis(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }
            return RedisWMS.GetRedis(key);
        }

        /// <summary>
        /// 清除复核缓存
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <param name="warehouseSysId"></param>
        [HttpGet, Route("CleanReviewRecords")]
        public void CleanReviewRecords(string outboundOrder, Guid warehouseSysId)
        {
            _redisAppService.CleanReviewRecords(outboundOrder, warehouseSysId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        [HttpGet, Route("SynchroSetAccessLog")]
        public void SynchroSetAccessLog(int type)
        {
            _redisAppService.SynchroSetAccessLog(type);
        }

        /// <summary>
        /// 根据key清除缓存
        /// </summary>
        /// <param name="key"></param>
        [HttpGet, Route("ClearRedisByKey")]
        public void ClearRedisByKey(string key)
        {
            _redisAppService.ClearRedisByKey(key);
        }


    }
}