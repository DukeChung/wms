using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Other;
using NBK.ECService.WMSReport.DTO.Query;

namespace NBK.ECService.WMSReport.Application.Interface
{
    public interface IOtherReadAppService : IApplicationService
    {
        #region 出库
        Pages<OutboundListDto> GetOutboundByPage(OutboundQuery request);

        OutboundViewDto GetOutboundBySysId(Guid sysid, Guid warehouseSysId);

        Pages<OutboundExceptionDto> GetOutboundDetailList(OutboundExceptionQueryDto request);

        /// <summary>
        /// 根据条件获取出库单信息
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        OutboundViewDto GetOutboundOrderByOrderId(OutboundQuery outboundQuery);

        /// <summary>
        /// 获取出库单预包装差异
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        OutboundPrePackDiffDto GetOutboundPrePackDiff(Guid outboundSysId, Guid warehouseSysId);

        /// <summary>
        /// 获取出库单散货箱差异
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        OutboundPrePackDiffDto GetOutboundPreBulkPackDiff(Guid outboundSysId, Guid wareHouseSysId);

        /// <summary>
        /// 获取出库单对应箱子信息
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        OutboundBoxListDto GetOutboundBox(Guid outboundSysId, Guid warehouseSysId);

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pickDetailQuery"></param>
        /// <returns></returns>
        Pages<PickDetailListDto> GetPickDetailListDtoByPageInfo(PickDetailQuery pickDetailQuery);

        /// <summary>
        /// 获取待拣货出库
        /// </summary>
        /// <param name="pickDetailQuery"></param>
        /// <returns></returns>
        Pages<PickOutboundListDto> GetPickOutboundListDtoByPageInfo(PickDetailQuery pickDetailQuery);

        /// <summary>
        /// 根据出库单ID获取异常明细
        /// </summary>
        /// <param name="sysid"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        List<OutboundExceptionDtoList> GetOutbooundExceptionData(Guid sysid, Guid warehouseSysId);
        #endregion

        #region 入库
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="purchaseQuery"></param>
        /// <returns></returns>
        Pages<PurchaseListDto> GetPurchaseDtoListByPageInfo(PurchaseQuery purchaseQuery);

        /// <summary>
        /// 退货入库
        /// </summary>
        /// <param name="purchaseQuery"></param>
        /// <returns></returns>
        Pages<PurchaseReturnListDto> GetPurchaseReturnDtoListByPageInfo(PurchaseReturnQuery purchaseQuery);


        /// <summary>
        /// 获取一条数据
        /// </summary>
        /// <param name="purchaseSysId"></param>
        /// <returns></returns>
        PurchaseViewDto GetPurchaseViewDtoBySysId(Guid purchaseSysId, Guid warehouseSysId);

        PurchaseReturnViewDto GetPurchaseReturnViewDtoBySysId(Guid purchaseSysId, Guid warehouseSysId);

        Pages<ReceiptListDto> GetReceiptList(ReceiptQuery receiptQuery);

        ReceiptViewDto GetReceiptViewById(Guid sysId, Guid warehouseSysId);

        /// <summary>
        /// 收货清单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        List<ReceiptDetailViewDto> GetReceiptDetailViewList(Guid sysId, Guid warehouseSysId);

        /// <summary>
        /// 收货批次清单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        List<ReceiptDetailViewDto> GetReceiptDetailLotViewList(Guid sysId, Guid warehouseSysId);

        /// <summary>
        /// 批次采集时获取收货清单明细
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        List<ReceiptDetailViewDto> GetReceiptDetailViewListByCollectionLot(Guid receiptSysId, Guid warehouseSysId);
        #endregion

        #region 库存转移
        /// <summary>
        /// 库存转移分页查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Pages<StockTransferLotListDto> GetStockTransferLotByPage(StockTransferQuery request);
        #endregion
    }
}
