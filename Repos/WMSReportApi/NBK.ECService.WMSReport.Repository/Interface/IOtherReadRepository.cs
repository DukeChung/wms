using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Other;
using NBK.ECService.WMSReport.DTO.Query;

namespace NBK.ECService.WMSReport.Repository.Interface
{
    public interface IOtherReadRepository : ICrudRepository
    {
        Pages<OutboundListDto> GetOutboundByPage(OutboundQuery request);

        OutboundViewDto GetOutboundBySysId(Guid outboundSysId, Guid wareHouseSysId);

        List<OutboundListDto> GetOutboundDetailBySummary(List<Guid> SysIds, Guid wareHouseSysId);

        List<OutboundDetailViewDto> GetOutboundDetails(Guid outboundSysId, Guid wareHouseSysId);

        Pages<OutboundExceptionDto> GetOutboundDetailList(OutboundExceptionQueryDto request);

        List<PrePackDetailDto> GetPrePackDetailByOutboundSysId(Guid outboundSysId, Guid wareHouseSysId);

        List<OutboundTransferOrderDetailDto> GetTransferOrderDetailBySysIds(List<Guid> transferOrderSysIds);

        Pages<PickDetailListDto> GetPickDetailListDtoByPageInfo(PickDetailQuery pickDetailQuery);

        List<PickDetailListDto> GetSummaryPickDetailListDto(List<Guid?> pickDetailSysIds, Guid wareHouseSysId);

        Pages<PickOutboundListDto> GetPickOutboundListDtoByPageInfo(PickDetailQuery pickDetailQuery);

        List<OutboundExceptionDtoList> GetOutbooundExceptionData(Guid sysId);

        List<PickOutboundDetailListDto> GetPickOutboundDetailListDto(List<Guid?> outboundSysIds, Guid wareHouseSysId);

        Pages<PurchaseListDto> GetPurchaseDtoListByPageInfo(PurchaseQuery purchaseQuery);

        Pages<PurchaseReturnListDto> GetPurchaseReturnDtoListByPageInfo(PurchaseReturnQuery purchaseQuery);

        PurchaseViewDto GetPurchaseViewDtoBySysId(Guid sysId, Guid wareHouseSysId);

        PurchaseReturnViewDto GetPurchaseReturnViewDtoBySysId(Guid sysId, Guid wareHouseSysId);

        List<PurchaseDetailViewDto> GetPurchaseDetailViewBySysId(Guid sysId, Guid wareHouseSysId);

        List<PurchaseDetailReturnViewDto> GetPurchaseDetailReturnViewBySysId(Guid sysId, Guid wareHouseSysId);

        /// <summary>
        /// 获取收货单列表
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        Pages<ReceiptListDto> GetReceiptListByPaging(ReceiptQuery receiptQuery);

        /// <summary>
        /// 根据Id获取收货单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        ReceiptViewDto GetReceiptViewById(Guid sysId, Guid wareHouseSysId);

        /// <summary>
        /// 获取收货清单明细
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        List<ReceiptDetailViewDto> GetReceiptDetailViewList(Guid receiptSysId, Guid wareHouseSysId);

        /// <summary>
        /// 获取收货批次清单明细
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        List<ReceiptDetailViewDto> GetReceiptDetailLotViewList(Guid receiptSysId, Guid wareHouseSysId);

        /// <summary>
        /// 批次采集时获取收货清单明细
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        List<ReceiptDetailViewDto> GetReceiptDetailViewListByCollectionLot(Guid receiptSysId);

        #region 库存转移
        /// <summary>
        /// 库存转移分页查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Pages<StockTransferLotListDto> GetStockTransferLotByPage(StockTransferQuery request);
        #endregion

        /// <summary>
        /// 获取出库单整件或者散件装箱数据
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        List<OutboundBoxDto> GetOutboundBox(Guid outboundSysId, Guid wareHouseSysId);
    }
}
