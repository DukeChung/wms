using System.Collections.Generic;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.DTO.InvLotLocLpn;
using NBK.ECService.WMS.Utility.Enum;
using System;
using NBK.ECService.WMS.DTO.MQ;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IWMSSqlRepository : ICrudRepository
    {
        /// <summary>
        /// 更新库存(上架)
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        CommonResponse UpdateInventoryQtyByShelves(List<UpdateInventoryDto> updateInventoryList);

        /// <summary>
        /// 更新分配库存
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        CommonResponse UpdateInventoryAllocatedQty(List<UpdateInventoryDto> updateInventoryList);

        /// <summary>
        /// 更新分配库存到拣货库存
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        CommonResponse UpdateInventoryPickedByAllocatedQty(List<UpdateInventoryDto> updateInventoryList);

        /// <summary>
        /// 更新拣货库存，发生实际财务库存扣减
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        CommonResponse UpdateInventoryQtyByPickedQty(List<UpdateInventoryDto> updateInventoryList);

        /// <summary>
        /// 快速发货
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        CommonResponse UpdateInventoryQuickDelivery(List<UpdateInventoryDto> updateInventoryList);

        /// <summary>
        /// 扣减库存方法(取消拣货)
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        CommonResponse UpdateInventoryCancelAllocatedQty(List<UpdateInventoryDto> updateInventoryList);

        /// <summary>
        /// 更新拣货库存数量(加工单拣货)
        /// </summary>
        /// <param name="updateInventoryDto"></param>
        CommonResponse UpdateInventoryAssemblyPickedQty(List<UpdateInventoryDto> updateInventoryList);

        /// <summary>
        /// 快速发货更新出库明细
        /// </summary>
        /// <param name="updateOutboundDto"></param>
        /// <returns></returns>
        CommonResponse UpdateOutboundDetailQuickDelivery(List<UpdateOutboundDto> updateOutboundDto);


        /// <summary>
        /// 快速发货更新出库明细
        /// </summary>
        /// <param name="updateOutboundDto"></param>
        /// <returns></returns>
        CommonResponse QuickDeliveryInsertPickDetail(List<pickdetail> pickdetails,string pickDetailOrder);

        /// <summary>
        /// 快速发货交易
        /// </summary>
        /// <param name="updateOutboundDto"></param>
        /// <returns></returns>
        CommonResponse QuickDeliveryInsertInvTrans(List<invtran> invtrans);

        /// <summary>
        /// 第三方接口调用写入出库单
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        CommonResponse ThirdPartyInsertOutboundDetail(List<outbounddetail> outbounddetails);

        /// <summary>
        /// 更新拣货库存数量(撤销领料)
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        CommonResponse UpdateInventoryCancelPickedQty(List<UpdateInventoryDto> updateInventoryList);

        /// <summary>
        /// 预包装明细更新
        /// </summary>
        /// <param name="updatePrePackDetailDtoList"></param>
        /// <returns></returns>
        CommonResponse UpdatePrePackDetail(List<UpdatePrePackDetailDto> updatePrePackDetailDtoList);

        #region 库存转移

        CommonResponse UpdateInventoryQtyByFromStockTransfer(List<UpdateInventoryDto> updateInventoryList);

        CommonResponse UpdateInventoryQtyByToStockTransfer(List<UpdateInventoryDto> updateInventoryList);

        void AddInventoryQtyByToStockTransfer(invlotloclpn invlotloclpn, invlot invlot, invskuloc invskuloc);
        /// <summary>
        /// 库存减少
        /// </summary>
        /// <param name="updateInventoryDto"></param>
        CommonResponse UpdateInventoryQtyByStockTransfer(List<UpdateInventoryDto> updateInventoryList);

        /// <summary>
        /// 更新库存(库存转移)
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        CommonResponse UpdateInventoryAddQtyByStockTransfer(List<UpdateInventoryDto> updateInventoryList);
        #endregion

        #region 分配发货
        /// <summary>
        /// 更新分配库存到财务库存发生扣减
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        CommonResponse UpdateInventoryAllocationDelivery(List<UpdateInventoryDto> updateInventoryList);

        /// <summary>
        /// 更新出库明细(分配发货)
        /// </summary>
        /// <param name="updateOutboundDto"></param>
        /// <returns></returns>
        CommonResponse UpdateOutboundDetailAllocationDelivery(List<UpdateOutboundDto> updateOutboundDto);

        /// <summary>
        /// 分配发货交易
        /// </summary>
        /// <param name="updateOutboundDto"></param>
        /// <returns></returns>
        CommonResponse AllocationDeliveryInsertInvTrans(List<invtran> invtrans);

        /// <summary>
        /// 分配发货 修改发货明细和拣货明细
        /// </summary>
        /// <returns></returns>
        CommonResponse UpdateOdAndPdByAllocationDelivery(UpdateOutboundDto updateOutboundDto, List<UpdateOutboundDetailDto> updateOutboundDetailDtoList);

        /// <summary>
        /// 部分发货，扣减剩余占用库存
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        CommonResponse UpdateInventoryByRemainingAllocatedQty(List<UpdateInventoryDto> updateInventoryList);
        #endregion

        /// <summary>
        /// 取消出库，或者退货入库，批量增加财务库存
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        CommonResponse UpdateInventoryForCancelOutbound(List<UpdateInventoryDto> updateInventoryList);

        /// <summary>
        /// 取消出库（正常出库）
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        CommonResponse UpdateInventoryForCancelOutboundShipment(List<UpdateInventoryDto> updateInventoryList);

        /// <summary>
        /// 取消出库 （更新出库明细信息，快速出库）
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="updateStatus"></param>
        /// <param name="userid"></param>
        /// <param name="userName"></param>
        void UpdateOutboundDetailForOutboundCancel_QuickDelivery(Guid outboundSysId, OutboundDetailStatus updateStatus, int userid, string userName);

        /// <summary>
        /// 取消出库 （更新出库明细信息,分配发货）
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="updateStatus"></param>
        /// <param name="userid"></param>
        /// <param name="userName"></param>
        /// <param name="allocatedQty"></param>
        void UpdateOutboundDetailForOutboundCancel_AllocationDelivery(Guid outboundSysId, OutboundDetailStatus updateStatus, int userid, string userName);

        /// <summary>
        /// 取消出库 （更新出库明细信息,正常发货）
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="updateStatus"></param>
        /// <param name="userid"></param>
        /// <param name="userName"></param>
        /// <param name="pickedQty"></param>
        void UpdateOutboundDetailForOutboundCancel_Shipment(Guid outboundSysId, OutboundDetailStatus updateStatus, int userid, string userName);

        void UpdatePickdetailForOutboundCancel_QuickDelivery(Guid outboundSysId, PickDetailStatus updateStatus, int userid, string userName);

        void UpdatePickdetailForOutboundCancel_AllocationDelivery(Guid outboundSysId, PickDetailStatus updateStatus, int userid, string userName);

        void UpdateInvtranForOutboundCancel_QuickDelivery(Guid docSysId, string updateStatus, int userid, string userName);

        void UpdateInvtranForOutboundCancel_AllocationDelivery(List<Guid> docSysIds, string updateStatus, int userid, string userName);

        #region  取消装箱
        /// <summary>
        /// 更新拣货库存到分配库存
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        CommonResponse UpdateInventoryCancelVanning(List<UpdateInventoryDto> updateInventoryList);
        #endregion

        /// <summary>
        /// 取消出库（分配出库）
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        CommonResponse UpdateInventoryForCancelOutboundAllocationDelivery(List<UpdateInventoryDto> updateInventoryList);

        /// <summary>
        /// 取消上架(扣减财务库存)
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        CommonResponse UpdateInventoryCancelShelves(List<UpdateInventoryDto> updateInventoryList);

        #region 取消收货
        /// <summary>
        /// 更新入库单，入库单明细，收货单明细
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        CommonResponse UpdateReceiptDetailCancelReceipt(ReceiptCancelDto receiptCancelDto);
        #endregion

        #region 库存冻结

        void UpdateInvForFrozenRequest(List<InvLotLocLpnDto> updateInventoryList, int updateId, string updateName);

        #endregion

        /// <summary>
        /// 退货入库，插入入库主表和批量插入入库单明细
        /// </summary>
        /// <param name="request"></param>
        void BatchInsertPurchaseAndDetails(purchase purchase,List<purchasedetail> details);

        /// <summary>
        /// 批量更新 库位 状态（冻结功能）
        /// </summary>
        /// <param name="zoneSysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <param name="status"></param>
        void BatchUpdateLocationStatusByZone(Guid zoneSysId, Guid warehouseSysId, LocationStatus status);
        /// <summary>
        /// 获取收货明细相同批次
        /// </summary>
        /// <param name="receiptDetails"></param>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        List<ReceiptDetailCheckLotDto> GetToLotByReceiptDetail(List<receiptdetail> receiptDetails, Guid purchaseSysId, Guid wareHouseSysId);

        /// <summary>
        /// 批量插入收货明细
        /// </summary>
        /// <param name="receiptDetails"></param>
        /// <returns></returns>
        CommonResponse BatchInsertReceiptDetail(List<receiptdetail> receiptDetails);

        /// <summary>
        /// 批量写入拣货
        /// </summary>
        /// <param name="pickdetail"></param>
        /// <returns></returns>
        CommonResponse BatchInsertPickDetail(List<pickdetail> pickdetail);

        /// <summary>
        /// 批量更新出库明细
        /// </summary>
        /// <param name="outbounddetailList"></param>
        /// <returns></returns>
        CommonResponse BatchUpdateOutboundDetail(List<outbounddetail> outbounddetailList);

        /// <summary>
        /// 收货完成更新入库单明细数量
        /// </summary>
        /// <param name="updatePurchaseDetailList"></param>
        /// <returns></returns>
        CommonResponse UpdatePurchaseDetailAfterReceipt(List<UpdatePurchaseDetailDto> updatePurchaseDetailList);

        /// <summary>
        /// 库存变更修改库存
        /// </summary>
        /// <param name="stockMovement"></param>
        /// <param name="currentUserId"></param>
        /// <param name="currentDisplayName"></param>
        /// <returns></returns>
        CommonResponse UpdateInventoryStockMovement(stockmovement stockMovement, int currentUserId, string currentDisplayName, bool isFromFrozen, bool isToFrozen);


        /// <summary>
        /// 自动上架更新收货明细
        /// </summary>
        /// <param name="updateReceiptDetails"></param>
        void UpdateReceiptDetailAfterAutoShelves(List<UpdateReceiptDetailDto> updateReceiptDetails);

        /// <summary>
        /// 批量插入invtrans
        /// </summary>
        /// <param name="invtrans"></param>
        void BatchInsertInvTrans(List<invtran> invtrans);

        /// <summary>
        /// 商品外借修改库存
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        bool UpdateInventoryBySkuBorrow(SkuBorrowDetailDto dto, Guid WareHouseSysId, int AuditingBy, string AuditingName, int status);

        /// <summary>
        /// 批量查询invlot
        /// </summary>
        /// <param name="inventorySkus"></param>
        /// <returns></returns>
        List<invlot> BatchGetInvLot(List<GetInventoryDto> inventorySkus);

        /// <summary>
        /// 批量查询invskuloc
        /// </summary>
        /// <param name="inventorySkus"></param>
        /// <returns></returns>
        List<invskuloc> BatchGetInvSkuLoc(List<GetInventoryDto> inventorySkus);

        /// <summary>
        /// 批量查询invlotloclpn
        /// </summary>
        /// <param name="inventorySkus"></param>
        /// <returns></returns>
        List<invlotloclpn> BatchGetInvLotLocLpn(List<GetInventoryDto> inventorySkus);

        /// <summary>
        /// 取消上架更新收货明细
        /// </summary>
        /// <param name="updateReceiptDetails"></param>
        /// <param name="currentUserId"></param>
        /// <param name="currentDisplayName"></param>
        void UpdateReceiptDetailAfterCancelShelves(List<UpdateReceiptDetailDto> updateReceiptDetails, int currentUserId, string currentDisplayName);

        #region 自增生成单号
        /// <summary>
        /// 自增生成单号
        /// </summary>
        /// <returns></returns>
        int AutoNextNumber();
        #endregion
        /// <summary>
        /// 取消上架批量更新交易表状态
        /// </summary>
        /// <param name="invTransSysIds"></param>
        /// <param name="currentUserId"></param>
        /// <param name="currentDisplayName"></param>
        void UpdateInvTransStatusAfterCancelShelves(IEnumerable<Guid> invTransSysIds, int currentUserId, string currentDisplayName);

        /// <summary>
        /// 批量插入invlot
        /// </summary>
        /// <param name="invLotList"></param>
        void BatchInsertInvLot(List<invlot> invLotList);

        /// <summary>
        /// 批量插入invlotloclpn
        /// </summary>
        /// <param name="invLotLocLpnList"></param>
        void BatchInsertInvLotLocLpn(List<invlotloclpn> invLotLocLpnList);

        /// <summary>
        /// 批量插入receiptdatarecord
        /// </summary>
        /// <param name="receiptDataRecordList"></param>
        void BatchInsertReceiptDataRecordList(List<receiptdatarecord> receiptDataRecordList);

        /// <summary>
        /// 批量插入invskuloc
        /// </summary>
        /// <param name="invSkuLocList"></param>
        void BatchInsertInvSkuLoc(List<invskuloc> invSkuLocList);

        /// <summary>
        /// 批量插入领料分拣记录
        /// </summary>
        /// <param name="pickingRecordsList"></param>
        void BatchInsertPickingRecords(List<pickingrecords> pickingRecordsList);

        /// <summary>
        /// 获取商品库位库存数量
        /// </summary>
        /// <param name="stockTakeDetails"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        List<InvSkuLocDto> GetInvQtyBySkuAndLoc(List<stocktakedetail> stockTakeDetails, Guid warehouseSysId);

        /// <summary>
        /// 修改散货装箱的出库单号
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="outboundOrder"></param>
        /// <param name="prePackSysId"></param>
        /// <returns></returns>
        CommonResponse UpdatePreBulkPackOutboundByBind(Guid outboundSysId, string outboundOrder, Guid prePackSysId);

        /// <summary>
        /// 修改散货装箱的出库单号（解绑）
        /// </summary>
        /// <param name="prePackSysId"></param>
        /// <returns></returns>
        CommonResponse UpdatePreBulkPackOutboundByUnBind(Guid prePackSysId);

        /// <summary>
        /// 损益审核更新库存
        /// </summary>
        /// <param name="updateInventoryList"></param>
        /// <returns></returns>
        CommonResponse UpdateInventoryAdjustmentAudit(List<UpdateInventoryDto> updateInventoryList);

        #region 工单相关
        /// <summary>
        /// 修改工单状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        CommonResponse UpdateWorkByDocOrder(MQWorkDto request);

        /// <summary>
        /// 修改出库单指派人
        /// </summary>
        /// <returns></returns>
        CommonResponse UpdateOutboundWorkName(List<WorkDetailDto> workDetailList);

        /// <summary>
        /// 修改收货单指派人
        /// </summary>
        /// <returns></returns>
        CommonResponse UpdateReceiptWorkName(List<WorkDetailDto> workDetailList);
        #endregion

        /// <summary>
        /// RF容器拣货更新PickDetail拣货数量
        /// </summary>
        /// <param name="updatePickDetailList"></param>
        /// <returns></returns>
        CommonResponse UpdatePickDetailRFContainerPicking(List<UpdatePickDetailDto> updatePickDetailList);

        /// <summary>
        /// 还原拣货容器
        /// </summary>
        /// <param name="clearContainerDto"></param>
        /// <returns></returns>
        CommonResponse ClearContainer(ClearContainerDto clearContainerDto);

        /// <summary>
        /// 更新出库明细备注
        /// </summary>
        /// <param name="partShipmentMemoDto"></param>
        /// <returns></returns>
        CommonResponse UpdateOutboundDetailMemo(PartShipmentMemoDto partShipmentMemoDto);

        /// <summary>
        /// 更新拣货单拣货数量
        /// </summary>
        /// <param name="pickingOperationDto"></param>
        /// <returns></returns>
        CommonResponse UpdatePickDetailPickedQty(PickingOperationDto pickingOperationDto);
    }
}