using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO.Account;
using NBK.ECService.WMS.DTO.System;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Redis;
using NBK.ECService.WMS.DTO;

namespace NBK.ECService.WMS.Application
{
    public class RedisAppService : ApplicationService, IRedisAppService
    {
        private ICrudRepository _crudRepository = null;
        public RedisAppService(ICrudRepository crudRepository)
        {
            this._crudRepository = crudRepository;
        }

        /// <summary>
        /// 同步菜单
        /// </summary>
        public void SynchroMenu()
        {
            RedisWMS.CleanRedis<menu>(RedisSourceKey.RedisMenuList);
            var menuList = _crudRepository.GetQuery<menu>(x => x.IsActive == true);
            var menuDto = menuList.JTransformTo<MenuDto>();
            RedisWMS.SetRedis(menuDto, RedisSourceKey.RedisMenuList);
        }

        public void CleanUserLoginRedis()
        {
            var loginUserList = RedisWMS.GetRedisList<List<LoginUserInfo>>(RedisSourceKey.RedisLoginUserList);
            if (loginUserList != null && loginUserList.Any())
            {
                for (int i = 0; loginUserList.Count > i; i++)
                {
                    if (loginUserList[i].UserName == null)
                    {
                        loginUserList.Remove(loginUserList[i]);
                    }
                }
                RedisWMS.SetRedis(loginUserList, RedisSourceKey.RedisLoginUserList);
            }
        }


        /// <summary>
        /// 包装和单位
        /// </summary>
        public void SynchroPack()
        {
            //RedisWMS.CleanRedis<pack>();
            //RedisWMS.CleanRedis<uom>();
            //var packList = _crudRepository.GetAll<pack>();
            //var uomList = _crudRepository.GetAll<uom>();
            //RedisWMS.SetRedis<pack>(packList);
            //RedisWMS.SetRedis<uom>(uomList);
        }

        /// <summary>
        /// 商品和商品分类
        /// </summary>

        public void SynchroSku()
        {
            //RedisWMS.CleanRedis<sku>();
            //RedisWMS.CleanRedis<skuclass>();
            //var skuList = _crudRepository.GetAll<sku>();
            //var skuclassList = _crudRepository.GetAll<skuclass>();
            //RedisWMS.SetRedis<sku>(skuList);
            //RedisWMS.SetRedis<skuclass>(skuclassList);
        }

        /// <summary>
        /// 同步批次模板
        /// </summary>

        public void SynchroLottemplate()
        {
            //RedisWMS.CleanRedis<lottemplate>(); 
            //var lottemplateList = _crudRepository.GetAll<lottemplate>();
            //RedisWMS.SetRedis<lottemplate>(lottemplateList);
        }

        /// <summary>
        /// 同步仓库
        /// </summary>

        public void SynchroWarehouse()
        {
            RedisWMS.CleanRedis<warehouse>(RedisSourceKey.RedisWareHouseList);
            var warehouseList = _crudRepository.GetAll<warehouse>(); 
            RedisWMS.SetRedis(warehouseList, RedisSourceKey.RedisWareHouseList);
        }

        /// <summary>
        /// 清除复核结果
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <param name="warehouseSysId"></param>
        public void CleanReviewRecords(string outboundOrder, Guid warehouseSysId)
        {
            RedisWMS.CleanRedis<List<RFOutboundDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewScanning, outboundOrder, warehouseSysId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public void SynchroSetAccessLog(int type)
        {
            RedisWMS.CleanRedis<bool>(RedisSourceKey.RedisSetAccessLog);
            RedisWMS.SetRedis(type, RedisSourceKey.RedisSetAccessLog);
        }

        /// <summary>
        /// 根据key清除缓存
        /// </summary>
        /// <param name="key"></param>
        public void ClearRedisByKey(string key)
        {
            RedisWMS.CleanRedis<bool>(key);
        }
    }
}