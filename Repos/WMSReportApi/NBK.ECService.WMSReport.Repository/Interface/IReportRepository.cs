using System;
using System.Collections.Generic;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Report;
using NBK.ECService.WMSReport.DTO.Other;
using NBK.ECService.WMSReport.DTO.Query;

namespace NBK.ECService.WMSReport.Repository.Interface
{
    public interface IReportRepository : ICrudRepository
    {
        /// <summary>
        /// 货卡变动
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="invTransBySkuReportQuery"></param>
        /// <returns></returns>
        Pages<InvTranDto> GetInvTranListByPaging(Guid skuSysId, InvTransBySkuReportQuery invTransBySkuReportQuery);


        /// <summary>
        /// 货卡变动
        /// </summary>
        /// <param name="skuUpc"></param>
        /// <param name="invTransBySkuReportQuery"></param>
        /// <returns></returns>
        Pages<InvTranDto> GetInvTranListBySkuUpc(string skuUpc, InvTransBySkuReportQuery invTransBySkuReportQuery);


        Pages<InvSkuLocReportDto> GetInvLocBySkuReport(InvSkuLocReportQuery request);

        Pages<InvSkuLotReportDto> GetInvLotBySkuReport(InvSkuLotReportQuery request);

        Pages<InvSkuLotLocLpnReportDto> GetInvLotLocLpnBySkuReport(InvSkuLotLocLpnReportQuery request);

        Pages<InvSkuLotReportDto> GetExpiryInvLotBySkuReport(InvSkuLotReportQuery request);

        Pages<ReceiptDetailReportDto> GetReceiptDetailReport(ReceiptDetailReportQuery request);

        Pages<OutboundDetailReportDto> GetOutboundDetailReport(OutboundDetailReportQuery request);

        Pages<InvSkuReportDto> GetInvSkuReport(InvSkuReportQuery request);

        Pages<FinanceInvoicingReportDto> GetFinanceInvoicingReport(FinanceInvoicingReportQueryDto request);

        Pages<AdjustmentDetailReportDto> GetAdjustmentDetailReport(AdjustmentDetailReportQuery request);

        /// <summary>
        /// 入库明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Pages<PurchaseDetailReportDto> GetPurchaseDetailReport(PurchaseDetailReportQuery request);

        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="invTransBySkuReportQuery"></param>
        /// <returns></returns>
        List<InvTransBySkuReportDto> GetSkuInfoByQuery(InvTransBySkuReportQuery invTransBySkuReportQuery);
        Pages<InboundReportDto> GetInboundReport(InboundReportQuery inboundReportQuery);

        Pages<SkuBorrowReportDto> GetSkuBorrowListByPage(SkuBorrowReportQuery query);

        Pages<FrozenSkuReportDto> GetFrozenSkuReport(FrozenSkuReportQuery request);

        Pages<ReceivedAndSendSkuReportDto> GetReceivedAndSendSkuReport(ReceivedAndSendSkuReportQuery request);

        Pages<OutboundHandleDateStatisticsDto> GetOutboundHandleDateStatisticsReport(OutboundHandleDateStatisticsQuery request);

        Pages<ReceiptAndDeliveryDateDto> GetReceiptAndDeliveryDateReport(ReceiptAndDeliveryDateQuery request);

        Pages<PurchaseMonitorDto> GetMonitorReportPurchaseDto(MonitorReportQuery request);

        Pages<OutboundMonitorDto> GetMonitorReportOutBoundDto(MonitorReportQuery request);

        Pages<WorkMonitorDto> GetMonitorReportWorkDto(MonitorReportQuery request);

        Pages<PurchaseReturnMonitorDto> GetMonitorReportPurchaseReturnDto(MonitorReportQuery request);

        Pages<StockTransferMonitorDto> GetMonitorReportStockTransferDto(MonitorReportQuery request);

        Pages<SNManageReportDto> GetSNManageReport(SNManageReportQuery request);

        /// <summary>
        /// 异常报告报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Pages<OutboundExceptionReportDto> GetOutboundExceptionReport(OutboundExceptionReportQuery request);

        /// <summary>
        /// 出库箱数统计报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Pages<OutboundBoxReportDto> GetOutboundBoxReport(OutboundBoxReportQuery request);

        /// <summary>
        /// 获取出库单整件或者散件装箱数据
        /// </summary>
        /// <param name="outboundSysIds"></param>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        List<OutboundBoxDto> GetOutboundBox(List<Guid> outboundSysIds, Guid wareHouseSysId);

        Pages<BalanceReportDto> GetBalanceReport(BalanceReportQuery request);

        Pages<OutboundPackReportDto> GetOutboundPackReport(OutboundPackReportQuery request);

        Pages<OutboundPackSkuReportDto> GetOutboundPackSkuReport(OutboundPackSkuReportQuery request);

        Pages<SkuPackReportListDto> GetSkuPackReport(SkuPackReportQuery request);

        Pages<OutboundListDto> GetOutboundSummaryReport(OutboundQuery request);

        Pages<ChannelInventoryDto> GetChannelInventoryByPage(ChannelInventoryQuery request);
    }
}