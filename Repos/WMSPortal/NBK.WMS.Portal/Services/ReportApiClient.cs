using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using NBK.ECService.WMS.DTO.Report;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBK.WMS.Portal.Services
{
    public class ReportApiClient
    {
        private static readonly ReportApiClient instance = new ReportApiClient();

        private ReportApiClient() { }

        public static ReportApiClient GetInstance() { return instance; }

        /// <summary>
        /// 货卡查询
        /// </summary>
        /// <param name="invTransBySkuReportQuery"></param>
        /// <returns></returns>
        public ApiResponse<InvTransSkuListReportDto> GetInvTransBySkuReport(CoreQuery query, InvTransBySkuReportQuery invTransBySkuReportQuery)
        {
            return ApiClient.Post<InvTransSkuListReportDto>(PublicConst.WMSReportUrl, "/Report/GetInvTransBySkuReport", query, invTransBySkuReportQuery);
        }

        /// <summary>
        /// 货位库存查询
        /// </summary>
        /// <param name="adjustmentQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<InvSkuLocReportDto>> GetInvLocBySkuReport(InvSkuLocReportQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<InvSkuLocReportDto>>(PublicConst.WMSReportUrl, "/Report/GetInvLocBySkuReport", query, request);
        }

        /// <summary>
        /// 批次库存查询
        /// </summary>
        /// <param name="adjustmentQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<InvSkuLotReportDto>> GetInvLotBySkuReport(InvSkuLotReportQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<InvSkuLotReportDto>>(PublicConst.WMSReportUrl, "/Report/GetInvLotBySkuReport", query, request);
        }

        /// <summary>
        /// 批次货位库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<InvSkuLotLocLpnReportDto>> GetInvLotLocLpnBySkuReport(InvSkuLotLocLpnReportQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<InvSkuLotLocLpnReportDto>>(PublicConst.WMSReportUrl, "/Report/GetInvLotLocLpnBySkuReport", query, request);
        }

        /// <summary>
        /// 临期批次库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<InvSkuLotReportDto>> GetExpiryInvLotBySkuReport(InvSkuLotReportQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<InvSkuLotReportDto>>(PublicConst.WMSReportUrl, "/Report/GetExpiryInvLotBySkuReport", query, request);
        }

        /// <summary>
        /// 收货明细
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<ReceiptDetailReportDto>> GetReceiptDetailReport(ReceiptDetailReportQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<ReceiptDetailReportDto>>(PublicConst.WMSReportUrl, "/Report/GetReceiptDetailReport", query, request);
        }

        /// <summary>
        /// 出库明细
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<OutboundDetailReportDto>> GetOutboundDetailReport(OutboundDetailReportQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<OutboundDetailReportDto>>(PublicConst.WMSReportUrl, "/Report/GetOutboundDetailReport", query, request);
        }

        public ApiResponse<UploadResultInformation> GetOutboundDetailReportByFile(OutboundDetailReportQuery request, CoreQuery query)
        {
            return ApiClient.Post<UploadResultInformation>(PublicConst.WMSReportUrl, "/Report/GetOutboundDetailReportByFile", query, request);
        }

        /// <summary>
        /// 查询库存商品数量
        /// </summary>
        /// <returns></returns>
        public ApiResponse<Pages<InvSkuReportDto>> GetInvSkuReport(InvSkuReportQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<InvSkuReportDto>>(PublicConst.WMSReportUrl, "/Report/GetInvSkuReport", query, request);
        }


        public ApiResponse<Pages<FinanceInvoicingReportDto>> GetFinanceInvoicingReport(
            FinanceInvoicingReportQueryDto request, CoreQuery query)
        {
            return ApiClient.Post<Pages<FinanceInvoicingReportDto>>(PublicConst.WMSReportUrl, "/Report/GetFinanceInvoicingReport", query, request);
        }

        public ApiResponse<Pages<AdjustmentDetailReportDto>> GetAdjustmentDetailReport(
                    AdjustmentDetailReportQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<AdjustmentDetailReportDto>>(PublicConst.WMSReportUrl, "/Report/GetAdjustmentDetailReport", query, request);
        }

        /// <summary>
        /// 图库明显报表查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<PurchaseDetailReportDto>> GetPurchaseDetailReport(PurchaseDetailReportQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<PurchaseDetailReportDto>>(PublicConst.WMSReportUrl, "/Report/GetPurchaseDetailReport", query, request);
        }
        public ApiResponse<Pages<InboundReportDto>> GetInboundReport(CoreQuery query, InboundReportQuery inboundReportQuery)
        {
            return ApiClient.Post<Pages<InboundReportDto>>(PublicConst.WMSReportUrl, "/Report/GetInboundReport", query, inboundReportQuery);
        }

        public ApiResponse<Pages<FrozenSkuReportDto>> GetFrozenSkuReport(CoreQuery query, FrozenSkuReportQuery request)
        {
            return ApiClient.Post<Pages<FrozenSkuReportDto>>(PublicConst.WMSReportUrl, "/Report/GetFrozenSkuReport", query, request);
        }

        public ApiResponse<UploadResultInformation> GetFrozenSkuReportByFile(FrozenSkuReportQuery request, CoreQuery query)
        {
            return ApiClient.Post<UploadResultInformation>(PublicConst.WMSReportUrl, "/Report/GetFrozenSkuReportByFile", query, request);
        }

        public ApiResponse<Pages<SNManageReportDto>> GetSNManageReport(CoreQuery query, SNManageReportQuery request)
        {
            return ApiClient.Post<Pages<SNManageReportDto>>(PublicConst.WMSReportUrl, "/Report/GetSNManageReport", query, request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<ReceivedAndSendSkuReportDto>> GetReceivedAndSendSkuReport(CoreQuery query, ReceivedAndSendSkuReportQuery request)
        {
            return ApiClient.Post<Pages<ReceivedAndSendSkuReportDto>>(PublicConst.WMSReportUrl, "/Report/GetReceivedAndSendSkuReport", query, request);
        }


        /// <summary>
        /// 出库处理时间统计表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<OutboundHandleDateStatisticsDto>> GetOutboundHandleDateStatisticsReport(CoreQuery query, OutboundHandleDateStatisticsQuery request)
        {
            return ApiClient.Post<Pages<OutboundHandleDateStatisticsDto>>(PublicConst.WMSReportUrl, "/Report/GetOutboundHandleDateStatisticsReport", query, request);
        }


        /// <summary>
        /// 收货、上架时间统计表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<ReceiptAndDeliveryDateDto>> GetReceiptAndDeliveryDateReport(CoreQuery query, ReceiptAndDeliveryDateQuery request)
        {
            return ApiClient.Post<Pages<ReceiptAndDeliveryDateDto>>(PublicConst.WMSReportUrl, "/Report/GetReceiptAndDeliveryDateReport", query, request);
        }


        public ApiResponse<Pages<PurchaseMonitorDto>> GetMonitorReportPurchaseDto(CoreQuery query, MonitorReportQuery request)
        {
            return ApiClient.Post<Pages<PurchaseMonitorDto>>(PublicConst.WMSReportUrl, "/Report/GetMonitorReportPurchaseDto", query, request);
        }

        public ApiResponse<Pages<OutboundMonitorDto>> GetMonitorReportOutBoundDto(CoreQuery query, MonitorReportQuery request)
        {
            return ApiClient.Post<Pages<OutboundMonitorDto>>(PublicConst.WMSReportUrl, "/Report/GetMonitorReportOutBoundDto", query, request);
        }

        public ApiResponse<Pages<WorkMonitorDto>> GetMonitorReportWorkDto(CoreQuery query, MonitorReportQuery request)
        {
            return ApiClient.Post<Pages<WorkMonitorDto>>(PublicConst.WMSReportUrl, "/Report/GetMonitorReportWorkDto", query, request);
        }

        public ApiResponse<Pages<PurchaseReturnMonitorDto>> GetMonitorReportPurchaseReturnDto(CoreQuery query, MonitorReportQuery request)
        {
            return ApiClient.Post<Pages<PurchaseReturnMonitorDto>>(PublicConst.WMSReportUrl, "/Report/GetMonitorReportPurchaseReturnDto", query, request);
        }

        public ApiResponse<Pages<StockTransferMonitorDto>> GetMonitorReportStockTransferDto(CoreQuery query, MonitorReportQuery request)
        {
            return ApiClient.Post<Pages<StockTransferMonitorDto>>(PublicConst.WMSReportUrl, "/Report/GetMonitorReportStockTransferDto", query, request);
        }

        public ApiResponse<Pages<OutboundExceptionReportDto>> GetOutboundExceptionReport(CoreQuery query, OutboundExceptionReportQuery request)
        {
            return ApiClient.Post<Pages<OutboundExceptionReportDto>>(PublicConst.WMSReportUrl, "/Report/GetOutboundExceptionReport", query, request);
        }

        public ApiResponse<Pages<OutboundBoxReportDto>> GetOutboundBoxReport(CoreQuery query, OutboundBoxReportQuery request)
        {
            return ApiClient.Post<Pages<OutboundBoxReportDto>>(PublicConst.WMSReportUrl, "/Report/GetOutboundBoxReport", query, request);
        }

        public ApiResponse<Pages<BalanceReportDto>> GetBalanceReport(CoreQuery query, BalanceReportQuery request)
        {
            return ApiClient.Post<Pages<BalanceReportDto>>(PublicConst.WMSReportUrl, "/Report/GetBalanceReport", query, request);
        }

        public ApiResponse<Pages<OutboundPackReportDto>> GetOutboundPackReport(CoreQuery query, OutboundPackReportQuery request)
        {
            return ApiClient.Post<Pages<OutboundPackReportDto>>(PublicConst.WMSReportUrl, "/Report/GetOutboundPackReport", query, request);
        }

        public ApiResponse<Pages<OutboundPackSkuReportDto>> GetOutboundPackSkuReport(CoreQuery query, OutboundPackSkuReportQuery request)
        {
            return ApiClient.Post<Pages<OutboundPackSkuReportDto>>(PublicConst.WMSReportUrl, "/Report/GetOutboundPackSkuReport", query, request);
        }

        /// <summary>
        /// 商品包装查询报表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<SkuPackReportListDto>> GetSkuPackReport(CoreQuery query, SkuPackReportQuery request)
        {
            return ApiClient.Post<Pages<SkuPackReportListDto>>(PublicConst.WMSReportUrl, "/Report/GetSkuPackReport", query, request);
        }


        /// <summary>
        /// 出库汇总报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<OutboundListDto>> GetOutboundByPage(OutboundQuery request)
        {
            var query = new CoreQuery();
            return ApiClient.Post<Pages<OutboundListDto>>(PublicConst.WMSReportUrl, "/Report/GetOutboundSummaryReport", query, request);
        }

        public ApiResponse<UploadResultInformation> OutboundSummaryExport(OutboundQuery request, CoreQuery query)
        {
            return ApiClient.Post<UploadResultInformation>(PublicConst.WMSReportUrl, "/Report/GetOutboundSummaryExport", query, request);
        }

        /// <summary>
        /// 渠道库存报表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<ChannelInventoryDto>> GetChannelInventoryByPage(ChannelInventoryQuery request)
        {
            var query = new CoreQuery();
            return ApiClient.Post<Pages<ChannelInventoryDto>>(PublicConst.WMSReportUrl, "/Report/GetChannelInventoryByPage", query, request);
        }
    }
}