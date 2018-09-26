using System.Collections.Generic;
using System.Web.Http;
using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.ThirdParty;
using System;
using System.Data.Entity.Validation;
using NBK.ECService.WMS.Core.WebApi.Filters;
using System.Data.Entity.Infrastructure;

namespace NBK.ECService.WMS.ApiController.OtherControllers
{
    /// <summary>
    /// 第三方接口
    /// </summary>
    [RoutePrefix("api/ThirdParty")]
    [AccessLog]
    public class ThirdPartyController : AbpApiController
    {
        private IThirdPartyAppService _thirdPartyAppService;

        /// <summary>
        /// 系统构造
        /// </summary>
        /// <param name="thirdPartyAppService"></param>
        public ThirdPartyController(IThirdPartyAppService thirdPartyAppService)
        {
            _thirdPartyAppService = thirdPartyAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void ThirdPartyAPI() { }

        /// <summary>
        /// 插入/更新SKU
        /// </summary>
        /// <param name="skuDto"></param>
        /// <returns></returns>
        [HttpPost, Route("InsertOrUpdateSku")]
        public CommonResponse InsertOrUpdateSku(ThirdPartySkuDto skuDto)
        {
            return _thirdPartyAppService.InsertOrUpdateSku(skuDto);
        }

        /// <summary>
        /// 插入/更新Pack
        /// </summary>
        /// <param name="skuDto"></param>
        /// <returns></returns>
        [HttpPost, Route("InsertOrUpdateSkuPack")]
        public CommonResponse InsertOrUpdateSkuPack(ThirdPartySkuPackDto skuPack)
        {
            return _thirdPartyAppService.InsertOrUpdateSkuPack(skuPack);
        }

        /// <summary>
        /// 插入/更新SKU分类
        /// </summary>
        /// <param name="skuClassDto"></param>
        /// <returns></returns>
        [HttpPost, Route("InsertOrUpdateSkuClass")]
        public CommonResponse InsertOrUpdateSkuClass(ThirdPartySkuClassDto skuClassDto)
        {
            return _thirdPartyAppService.InsertOrUpdateSkuClass(skuClassDto);
        }

        /// <summary>
        /// 插入采购单
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        [HttpPost, Route("InsertPurchase")]
        public CommonResponse InsertPurchase(ThirdPartyPurchaseDto purchaseDto)
        {
            try
            {
                return _thirdPartyAppService.InsertPurchase(purchaseDto);
            }

            catch (DbEntityValidationException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 插入订单
        /// </summary>
        /// <param name="outboundDto"></param>
        /// <returns></returns>
        [HttpPost, Route("InsertOutbound")]
        public CommonResponse InsertOutbound(ThirdPartyOutboundDto outboundDto)
        {
            return _thirdPartyAppService.InsertOutbound(outboundDto);
        }

        /// <summary>
        /// 商城入库接口
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpGet, Route("InsertInStock")]
        public CommonResponse InsertInStock(Guid sysId, int currentUserId, string currentDisplayName)
        {
            return _thirdPartyAppService.InsertInStock(currentUserId, currentDisplayName, sysId);
        }

        /// <summary>
        /// 商城出库接口
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpGet, Route("InsertOutStock")]
        public CommonResponse InsertOutStock(Guid sysId, string currentUserName, int currentUserId)
        {
            return _thirdPartyAppService.InsertOutStock(sysId, currentUserName, currentUserId);
        }

        /// <summary>
        /// 关闭出库单
        /// </summary>
        /// <param name="orderCode"></param>
        /// <param name="currentUserId"></param>
        /// <param name="currentUserName"></param>
        /// <returns></returns>
        [HttpGet, Route("VoidOutbound")]
        public CommonResponse VoidOutbound(string orderCode, string wareHouseId, int currentUserId, string currentUserName)
        {
            return _thirdPartyAppService.VoidOutbound(orderCode, wareHouseId, currentUserId, currentUserName);
        }

        /// <summary>
        /// 写入仓库数据
        /// </summary>
        /// <param name="thirdPartyWareHouseDto"></param>
        /// <returns></returns>
        [HttpPost, Route("InsertWareHouse")]
        public CommonResponse InsertWareHouse(ThirdPartyWareHouseDto thirdPartyWareHouseDto)
        {
            return _thirdPartyAppService.InsertWareHouse(thirdPartyWareHouseDto);
        }

        /// <summary>
        /// 组装单
        /// </summary>
        /// <param name="assemblyDto"></param>
        /// <returns></returns>
        [HttpPost, Route("InsertAssembly")]
        public CommonResponse InsertAssembly(ThirdPartyAssemblyDto assemblyDto)
        {
            return _thirdPartyAppService.InsertAssembly(assemblyDto);
        }

        /// <summary>
        /// 关闭采购单
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        [HttpPost, Route("ClosePurchase")]
        public CommonResponse ClosePurchase(ThirdPartyPurchaseOperateDto purchaseDto)
        {
            try
            {
                return _thirdPartyAppService.ClosePurchase(purchaseDto);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("操作失败");
            }
        }

        /// <summary>
        /// ecc调用:创建移仓单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("CreateTransferInventory")]
        public CommonResponse CreateTransferInventory(ThirdPartTransferInventoryDto request)
        {
            try
            {
                return _thirdPartyAppService.CreateTransferInventory(request);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("操作失败");
            }
        }

        /// <summary>
        /// 通过名称查询地区列表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet, Route("GetRegionListByName")]
        public Dictionary<string, string> GetRegionListByName(string name)
        {
            return _thirdPartyAppService.GetRegionListByName(name);
        }

        /// <summary>
        /// 获取详细地址
        /// </summary>
        /// <param name="regionSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetRegionIntactBySysId")]
        public string GetRegionIntactBySysId(Guid regionSysId)
        {
            return _thirdPartyAppService.GetRegionIntactBySysId(regionSysId);
        }

        /// <summary>
        /// ecc调用:库存转移
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("InsertStockTransfer")]
        public CommonResponse InsertStockTransfer(ThirdPartyStockTransferDto request)
        {
            return _thirdPartyAppService.InsertStockTransfer(request);
        }

        /// <summary>
        /// 渠道库存初始化
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("GetInitChannelInventoryData")]
        public List<ThirdPartyStockTransferDto> GetInitChannelInventoryData(InitInventoryFromChannelRequest request)
        {
            return _thirdPartyAppService.GetInitChannelInventoryData(request);
        }

        /// <summary>
        /// TMS调用：装车顺序
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("InsertLoadingSequence")]
        public CommonResponse InsertLoadingSequence(ThirdPartyLoadingSequenceDto request)
        {
            return _thirdPartyAppService.InsertLoadingSequence(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("InsertTMSOrder")]
        public CommonResponse InsertTMSOrder(ThirdPartyLoadingSequenceDto request)
        {
            return _thirdPartyAppService.InsertTMSOrder(request);
        }


        /// <summary>
        /// ecc调用:商品外借新增
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("InsertSkuBorrow")]
        public CommonResponse InsertSkuBorrow(ThirdPartySkuBorrowAddDto request)
        {
            return _thirdPartyAppService.InsertSkuBorrow(request);
        }


        ///// <summary>
        ///// ecc调用:商品外借借出
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[HttpPost, Route("PushLendInfoToECC")]
        //public CommonResponse PushLendInfoToECC(ThirdPartySkuBorrowLendDto request)
        //{
        //    return _thirdPartyAppService.PushLendInfoToECC(request);
        //}

        ///// <summary>
        ///// ecc调用:商品外借归还
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[HttpPost, Route("PushReturnInfoToECC")]
        //public CommonResponse PushReturnInfoToECC(ThirdPartySkuBorrowReturnDto request)
        //{
        //    return _thirdPartyAppService.PushReturnInfoToECC(request);
        //}

        /// <summary>
        /// ecc调用:已出库单据创建退货单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("InsertReturnPurchase")]
        public CommonResponse InsertReturnPurchase(ThirdPartyReturnPurchaseDto request)
        {
            return _thirdPartyAppService.InsertReturnPurchase(request);
        }


        /// <summary>
        /// ecc调用:待出库单据更新出库单
        /// </summary>
        /// <param name="thirdPartyUpdateOutboundDto"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateOutboundByReturnPurchase")]
        public CommonResponse UpdateOutboundByReturnPurchase(ThirdPartyUpdateOutboundDto thirdPartyUpdateOutboundDto)
        {
            return _thirdPartyAppService.UpdateOutboundByReturnPurchase(thirdPartyUpdateOutboundDto);
        }

    }
}