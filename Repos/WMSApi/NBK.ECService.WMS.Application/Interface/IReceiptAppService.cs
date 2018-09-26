using System;
using System.Collections.Generic;
using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.DTO.Receipt;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IReceiptAppService : IApplicationService
    {
        Pages<ReceiptListDto> GetReceiptList(ReceiptQuery receiptQuery);

        ReceiptOperationDto GetReceiptOperationByOrderNumber(string orderNumber,string currentUserName, int currentUserId, Guid currentWarehouseSysId);

        ReceiptOperationDto CreateReceiptByPoOrder(string orderNumber, string currentUserName, int currentUserId, Guid warehouseSysId);

        string SaveReceiptOperation(ReceiptOperationDto receiptOperationDto);

        void UpdateReceiptStatus(Guid sysId, ReceiptStatus status, string currentUserName, int currentUserId, Guid warehouseSysId);

        /// <summary>
        /// 根据条件获取收货单信息
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        ReceiptDto GetReceiptOrderByOrderId(ReceiptQuery receiptQuery);

        #region 取消收货
        /// <summary>
        /// 取消收货（入库单整单取消）
        /// </summary>
        /// <param name="receiptCancelDto"></param>
        /// <returns></returns>
        CommonResponse CancelReceiptByPurchase(ReceiptCancelDto receiptCancelDto);
        #endregion

        #region 领料分拣
        /// <summary>
        /// 领料分拣
        /// </summary>
        /// <param name="picking"></param>
        /// <returns></returns>
        CommonResponse PickingMaterial(PickingMaterialDto picking);

        /// <summary>
        /// 获取领料记录
        /// </summary>
        /// <param name="pickingQuery"></param>
        /// <returns></returns>
        Pages<PickingMaterialListDto> GetPickingMaterialList(PickingMaterialQuery pickingQuery);
        #endregion

        CheckDuplicateSNDto CheckDuplicateSN(List<string> snList,string type, Guid warehouseSysId);

        #region 批次采集
        /// <summary>
        /// 根据商品获取批次采集相关信息
        /// </summary>
        /// <param name="upc"></param>
        /// <param name="skuSysId"></param>
        /// <returns></returns>
        ReceiptCollectionLotViewDto GetReceiptDetailCollectionLotViewList(ReceiptCollectionLotQuery request);

        /// <summary>
        /// 保存批次采集
        /// </summary>
        /// <param name="receiptOperationDto"></param>
        /// <returns></returns>
        ReceiptDetailResponseDto SaveReceiptDetailLot(ReceiptCollectionLotDto receiptCollectionLotDto);
        #endregion
    }
}