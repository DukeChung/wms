using System;
using System.Collections.Generic;
using System.Web.Http;
using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Core.WebApi.Filters;
using System.Data.Entity.Infrastructure;
using NBK.ECService.WMS.DTO.Receipt;

namespace NBK.ECService.WMS.ApiController.InboundControllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/Inbound/Receipt")]
    [AccessLog]
    public class ReceiptController : AbpApiController
    {
        private IReceiptAppService _receiptAppService;

        /// <summary>
        /// 系统构造
        /// </summary>
        /// <param name="receiptAppService"></param>
        public ReceiptController(IReceiptAppService receiptAppService)
        {
            _receiptAppService = receiptAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void ReceiptApi()
        {
        }


        /// <summary>
        /// 获取收货操作信息
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReceiptOperationByOrderNumber")]
        public ReceiptOperationDto GetReceiptOperationByOrderNumber(string orderNumber, string currentUserName, int currentUserId, Guid currentWarehouseSysId)
        {
            return _receiptAppService.GetReceiptOperationByOrderNumber(orderNumber, currentUserName, currentUserId, currentWarehouseSysId);
        }

        /// <summary>
        /// 创建收货信息
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>z
        [HttpPost, Route("CreateReceiptByPoOrder")]
        public ReceiptOperationDto CreateReceiptByPoOrder(string orderNumber, string currentUserName, int currentUserId,Guid warehouseSysId)
        {
            return _receiptAppService.CreateReceiptByPoOrder(orderNumber, currentUserName, currentUserId,warehouseSysId);
        }

        /// <summary>
        /// 保存入库单明细信息
        /// </summary>
        /// <param name="receiptOperationDto"></param>
        /// <returns></returns>
        [HttpPost, Route("SaveReceiptOperation")]
        public string SaveReceiptOperation(ReceiptOperationDto receiptOperationDto)
        {
            try
            {
                return _receiptAppService.SaveReceiptOperation(receiptOperationDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("操作失败!");
            }
        }

        /// <summary>
        /// 更新入库单状态
        /// </summary>
        [HttpPost, Route("UpdateReceiptStatus")]
        public void UpdateReceiptStatus(Guid sysId, ReceiptStatus status, string currentUserName, int currentUserId,Guid warehouseSysId)
        {
            _receiptAppService.UpdateReceiptStatus(sysId, status, currentUserName, currentUserId,warehouseSysId);
        }

        #region 取消收货
        /// <summary>
        /// 取消收货（入库单整单取消）
        /// </summary>
        /// <param name="receiptCancelDto"></param>
        /// <returns></returns>
        [HttpPost, Route("CancelReceiptByPurchase")]
        public CommonResponse CancelReceiptByPurchase(ReceiptCancelDto receiptCancelDto)
        {
            return _receiptAppService.CancelReceiptByPurchase(receiptCancelDto);
        }
        #endregion

        #region 领料分拣
        /// <summary>
        /// 领料分拣
        /// </summary>
        /// <param name="picking"></param>
        /// <returns></returns>
        [HttpPost, Route("PickingMaterial")]
        public CommonResponse PickingMaterial(PickingMaterialDto picking)
        {
            try
            {
                return _receiptAppService.PickingMaterial(picking);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取领料记录
        /// </summary>
        /// <param name="pickingQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPickingMaterialList")]
        public Pages<PickingMaterialListDto> GetPickingMaterialList(PickingMaterialQuery pickingQuery)
        {
            return _receiptAppService.GetPickingMaterialList(pickingQuery);
        }
        #endregion


        /// <summary>
        /// 收货时检查SN是否重复
        /// </summary>
        /// <param name="snList"></param>
        /// <param name="type">校验场景类型: 1. 收货  2. 发货</param>
        /// <param name="warehouseSysId"></param>
        [HttpPost, Route("CheckDuplicateSN")]
        public CheckDuplicateSNDto CheckDuplicateSN(List<string> snList, string type ,Guid warehouseSysId)
        {
            return _receiptAppService.CheckDuplicateSN(snList, type,warehouseSysId);
        }

        #region 批次采集
        /// <summary>
        /// 根据商品获取批次采集相关信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReceiptDetailCollectionLotViewList")]
        public ReceiptCollectionLotViewDto GetReceiptDetailCollectionLotViewList(ReceiptCollectionLotQuery request)
        {
            return _receiptAppService.GetReceiptDetailCollectionLotViewList(request);
        }

        /// <summary>
        /// 保存批次采集
        /// </summary>
        /// <param name="receiptCollectionLotDto"></param>
        /// <returns></returns>
        [HttpPost, Route("SaveReceiptDetailLot")]
        public ReceiptDetailResponseDto SaveReceiptDetailLot(ReceiptCollectionLotDto receiptCollectionLotDto)
        {
            return _receiptAppService.SaveReceiptDetailLot(receiptCollectionLotDto);
        }
        #endregion
    }
}