using System;
using System.Collections.Generic;
using System.Web.Http;
using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.ApiController.InboundControllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/Inbound/Purchase")]
    [AccessLog]
    public class PurchaseController : AbpApiController
    {
        private IPurchaseAppService _purchaseAppService;

        /// <summary>
        /// 系统构造
        /// </summary>
        /// <param name="purchaseAppService"></param>
        public PurchaseController(IPurchaseAppService purchaseAppService)
        {
            _purchaseAppService = purchaseAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void PurchaseAPI()
        {
        }


        /// <summary>
        /// 更新入库单状态
        /// </summary>
        [HttpPost, Route("GetPurchaseDetailSkuByUpcIsNull")]
        public List<PurchaseDetailSkuDto> GetPurchaseDetailSkuByUpcIsNull(Guid purchaseSysId,Guid warehouseSysId)
        {
            return _purchaseAppService.GetPurchaseDetailSkuByUpcIsNull(purchaseSysId, warehouseSysId);
        }

        /// <summary>
        /// 更新Sku属性
        /// </summary>
        [HttpPost, Route("SavePurchaseDetailSkuStyle")]
        public void SavePurchaseDetailSkuStyle(PurchaseDetailSkuDto purchaseDetailSkuDto)
        {
             _purchaseAppService.SavePurchaseDetailSkuStyle(purchaseDetailSkuDto);
        }

        /// <summary>
        /// 生成采购单并收货
        /// </summary>
        [HttpPost, Route("SaveBatchPurchaseAndReceipt")]
        public void SaveBatchPurchaseAndReceipt(PurchaseBatchDto purchaseBatchDto)
        {
            _purchaseAppService.SaveBatchPurchaseAndReceipt(purchaseBatchDto);
        }

        /// <summary>
        /// 指定入库批号
        /// </summary>
        [HttpPost, Route("AppointBatchNumber")]
        public void AppointBatchNumber(string sysId, string batchNumber, Guid warehouseSysId)
        {
            _purchaseAppService.AppointBatchNumber(sysId.ToGuidList(),batchNumber,warehouseSysId);
        }

        /// <summary>
        /// 作废采购订单
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        [HttpPost, Route("ObsoletePurchase")]
        public bool ObsoletePurchase(PurchaseOperateDto purchaseDto)
        {
            return _purchaseAppService.ObsoletePurchase(purchaseDto);
        }

        /// <summary>
        /// 关闭采购订单
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        [HttpPost, Route("ClosePurchase")]
        public bool ClosePurchase(PurchaseOperateDto purchaseDto)
        {
            return _purchaseAppService.ClosePurchase(purchaseDto);
        }

        /// <summary>
        /// 退货入库，自动收货上架
        /// </summary>
        /// <param name="purchaseDto"></param>
        [HttpPost, Route("AutoShelves")]
        public void AutoShelves(PurchaseOperateDto purchaseDto)
        {
            _purchaseAppService.AutoShelves(purchaseDto);
        }

        /// <summary>
        /// 入库单生成质检单
        /// </summary>
        /// <param name="purchaseQcDto"></param>
        /// <returns></returns>
        [HttpPost, Route("GenerateQcOrderByPurchase")]
        public bool GenerateQcOrderByPurchase(PurchaseQcDto purchaseQcDto)
        {
            return _purchaseAppService.GenerateQcOrderByPurchase(purchaseQcDto);
        }

        /// <summary>
        /// 修改业务类型（指定上下行）
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="businessType"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdatePurchaseBusinessTypeBySysId")]
        public bool UpdatePurchaseBusinessTypeBySysId(string sysId, string businessType,Guid warehouseSysId)
        {
            return _purchaseAppService.UpdatePurchaseBusinessTypeBySysId(sysId.ToGuidList(), businessType, warehouseSysId);
        }

        /// <summary>
        /// 退货入库创建指定仓库入库单
        /// </summary>
        /// <param name="purchase"></param>
        [HttpPost, Route("InsertPurchaseAndDetailsByReturnOutbound")]
        public void InsertPurchaseAndDetailsByReturnOutbound(PurchaseForReturnDto purchase)
        {
            _purchaseAppService.InsertPurchaseAndDetailsByReturnOutbound(purchase);
        }
    }
}