using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Other;
using NBK.ECService.WMSReport.DTO.Query;
using NBK.ECService.WMSReport.DTO.Report;
using NBK.ECService.WMSReport.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS.Global.Portal.Services
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
            return ApiClient.Post<InvTransSkuListReportDto>(PublicConst.WmsReportApiUrl, "/Report/GetInvTransBySkuReport", query, invTransBySkuReportQuery);
        }

        /// <summary>
        /// 货位库存查询
        /// </summary>
        /// <param name="adjustmentQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<InvSkuLocGlobalDto>> GetInvLocBySkuReport(InvSkuLocGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<InvSkuLocGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetInvLocBySkuReport", query, request);
        }

        /// <summary>
        /// 批次库存查询
        /// </summary>
        /// <param name="adjustmentQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<InvSkuLotGlobalDto>> GetInvLotBySkuReport(InvSkuLotGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<InvSkuLotGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetInvLotBySkuReport", query, request);
        }

        /// <summary>
        /// 批次货位库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<InvSkuLotLocLpnGlobalDto>> GetInvLotLocLpnBySkuReport(InvSkuLotLocLpnGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<InvSkuLotLocLpnGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetInvLotLocLpnBySkuReport", query, request);
        }

        /// <summary>
        /// 临期批次库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<InvSkuLotGlobalDto>> GetExpiryInvLotBySkuReport(InvSkuLotGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<InvSkuLotGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetExpiryInvLotBySkuReport", query, request);
        }

        /// <summary>
        /// 收货明细
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<ReceiptDetailGlobalDto>> GetReceiptDetailReport(ReceiptDetailGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<ReceiptDetailGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetReceiptDetailReport", query, request);
        }

        /// <summary>
        /// 出库明细
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<OutboundDetailGlobalDto>> GetOutboundDetailReport(OutboundDetailGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<OutboundDetailGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetOutboundDetailReport", query, request);
        }

        public ApiResponse<UploadResultInformation> GetOutboundDetailReportByFile(OutboundDetailReportQuery request, CoreQuery query)
        {
            return ApiClient.Post<UploadResultInformation>(PublicConst.WmsReportApiUrl, "/Global/GetOutboundDetailReportByFile", query, request);
        }

        /// <summary>
        /// 查询库存商品数量
        /// </summary>
        /// <returns></returns>
        public ApiResponse<Pages<InvSkuGlobalDto>> GetInvSkuReport(InvSkuGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<InvSkuGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetInvSkuReport", query, request);
        }


        public ApiResponse<Pages<FinanceInvoicingGlobalDto>> GetFinanceInvoicingReport(FinanceInvoicingGlobalQueryDto request, CoreQuery query)
        {
            return ApiClient.Post<Pages<FinanceInvoicingGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetFinanceInvoicingReport", query, request);
        }

        public ApiResponse<Pages<AdjustmentDetailGlobalDto>> GetAdjustmentDetailReport(AdjustmentDetailGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<AdjustmentDetailGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetAdjustmentDetailReport", query, request);
        }

        /// <summary>
        /// 入库明细报表查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<PurchaseDetailGlobalDto>> GetPurchaseDetailReport(PurchaseDetailGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<PurchaseDetailGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetPurchaseDetailReport", query, request);
        }
        public ApiResponse<Pages<InboundGlobalDto>> GetInboundReport(CoreQuery query, InboundGlobalQuery inboundReportQuery)
        {
            return ApiClient.Post<Pages<InboundGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetInboundReport", query, inboundReportQuery);
        }

        public ApiResponse<Pages<FrozenSkuGlobalDto>> GetFrozenSkuReport(CoreQuery query, FrozenSkuGlobalQuery request)
        {
            return ApiClient.Post<Pages<FrozenSkuGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetFrozenSkuReport", query, request);
        }

        public ApiResponse<Pages<SNManageGlobalDto>> GetSNManageReport(CoreQuery query, SNManageGlobalQuery request)
        {
            return ApiClient.Post<Pages<SNManageGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetSNManageReport", query, request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<ReceivedAndSendSkuGlobalDto>> GetReceivedAndSendSkuReport(CoreQuery query, ReceivedAndSendSkuGlobalQuery request)
        {
            return ApiClient.Post<Pages<ReceivedAndSendSkuGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetReceivedAndSendSkuReport", query, request);
        }


        /// <summary>
        /// 出库处理时间统计表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<OutboundHandleDateStatisticsGlobalDto>> GetOutboundHandleDateStatisticsReport(CoreQuery query, OutboundHandleDateStatisticsGlobalQuery request)
        {
            return ApiClient.Post<Pages<OutboundHandleDateStatisticsGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetOutboundHandleDateStatisticsReport", query, request);
        }


        /// <summary>
        /// 收货、上架时间统计表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<ReceiptAndDeliveryDateGlobalDto>> GetReceiptAndDeliveryDateReport(CoreQuery query, ReceiptAndDeliveryDateGlobalQuery request)
        {
            return ApiClient.Post<Pages<ReceiptAndDeliveryDateGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetReceiptAndDeliveryDateReport", query, request);
        }


        public ApiResponse<Pages<PurchaseMonitorDto>> GetMonitorReportPurchaseDto(CoreQuery query, MonitorReportQuery request)
        {
            return ApiClient.Post<Pages<PurchaseMonitorDto>>(PublicConst.WmsReportApiUrl, "/Report/GetMonitorReportPurchaseDto", query, request);
        }

        public ApiResponse<Pages<OutboundMonitorDto>> GetMonitorReportOutBoundDto(CoreQuery query, MonitorReportQuery request)
        {
            return ApiClient.Post<Pages<OutboundMonitorDto>>(PublicConst.WmsReportApiUrl, "/Report/GetMonitorReportOutBoundDto", query, request);
        }

        public ApiResponse<Pages<WorkMonitorDto>> GetMonitorReportWorkDto(CoreQuery query, MonitorReportQuery request)
        {
            return ApiClient.Post<Pages<WorkMonitorDto>>(PublicConst.WmsReportApiUrl, "/Report/GetMonitorReportWorkDto", query, request);
        }

        public ApiResponse<Pages<PurchaseReturnMonitorDto>> GetMonitorReportPurchaseReturnDto(CoreQuery query, MonitorReportQuery request)
        {
            return ApiClient.Post<Pages<PurchaseReturnMonitorDto>>(PublicConst.WmsReportApiUrl, "/Report/GetMonitorReportPurchaseReturnDto", query, request);
        }

        public ApiResponse<Pages<StockTransferMonitorDto>> GetMonitorReportStockTransferDto(CoreQuery query, MonitorReportQuery request)
        {
            return ApiClient.Post<Pages<StockTransferMonitorDto>>(PublicConst.WmsReportApiUrl, "/Report/GetMonitorReportStockTransferDto", query, request);
        }

        public ApiResponse<Pages<OutboundExceptionGlobalDto>> GetOutboundExceptionReport(CoreQuery query, OutboundExceptionGlobalQuery request)
        {
            return ApiClient.Post<Pages<OutboundExceptionGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetOutboundExceptionReport", query, request);
        }

        public ApiResponse<Pages<OutboundBoxGlobalDto>> GetOutboundBoxReport(CoreQuery query, OutboundBoxGlobalQuery request)
        {
            return ApiClient.Post<Pages<OutboundBoxGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetOutboundBoxReport", query, request);
        }

        public ApiResponse<Pages<BalanceGlobalDto>> GetBalanceReport(CoreQuery query, BalanceGlobalQuery request)
        {
            return ApiClient.Post<Pages<BalanceGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetBalanceReport", query, request);
        }

        public ApiResponse<Pages<OutboundPackGlobalDto>> GetOutboundPackReport(CoreQuery query, OutboundPackGlobalQuery request)
        {
            return ApiClient.Post<Pages<OutboundPackGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetOutboundPackReport", query, request);
        }

        public ApiResponse<Pages<OutboundPackSkuGlobalDto>> GetOutboundPackSkuReport(CoreQuery query, OutboundPackSkuGlobalQuery request)
        {
            return ApiClient.Post<Pages<OutboundPackSkuGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetOutboundPackSkuReport", query, request);
        }

        /// <summary>
        /// 商品包装查询报表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<SkuPackGlobalListDto>> GetSkuPackReport(CoreQuery query, SkuPackGlobalQuery request)
        {
            return ApiClient.Post<Pages<SkuPackGlobalListDto>>(PublicConst.WmsReportApiUrl, "/Global/GetSkuPackReport", query, request);
        }


        /// <summary>
        /// 出库汇总报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<OutboundListGlobalDto>> GetOutboundByPage(OutboundGlobalQuery request)
        {
            var query = new CoreQuery();
            return ApiClient.Post<Pages<OutboundListGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetOutboundSummaryReport", query, request);
        }

        public ApiResponse<UploadResultInformation> OutboundSummaryExport(OutboundGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<UploadResultInformation>(PublicConst.WmsReportApiUrl, "/Global/GetOutboundSummaryExport", query, request);
        }

        public ApiResponse<Pages<TransferinventoryGlobalDto>> GetTransferinventoryReport(TransferinventoryGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<TransferinventoryGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetTransferinventoryReport", query, request);
        }

        /// <summary>
        /// 出库单商品汇总报表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<OutboundSkuGlobalDto>> GetOutboundSkuReport(OutboundSkuGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<OutboundSkuGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetOutboundSkuReport", query, request);
        }

        /// <summary>
        /// 出库捡货工时报表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<PickingTimeSpanGlobalDto>> GetPickingTimeSpanReport(PickingTimeSpanGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<PickingTimeSpanGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetPickingTimeSpanReport", query, request);
        }

        /// <summary>
        /// 出库复核工时报表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<OutboundTransferOrderGlobalDto>> GetOutboundTransferOrderReport(OutboundTransferOrderGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<OutboundTransferOrderGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetOutboundTransferOrderReport", query, request);
        }

        /// <summary>
        /// 退货单报表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<ReturnOrderGlobalDto>> GetReturnOrderReport(ReturnOrderGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<ReturnOrderGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetReturnOrderGlobalReport", query, request);
        }

        public ApiResponse<UploadResultInformation> GetReturnOrderReportExport(ReturnOrderGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<UploadResultInformation>(PublicConst.WmsReportApiUrl, "/Global/GetReturnOrderReportExport", query, request);
        }

        public ApiResponse<UploadResultInformation> GetPickingTimeSpanReportExport(PickingTimeSpanGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<UploadResultInformation>(PublicConst.WmsReportApiUrl, "/Global/GetPickingTimeSpanReportExport", query, request);
        }

        /// <summary>
        /// 农资出库商品报表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<FertilizerOutboundSkuGlobalDto>> GetFertilizerOutboundSkuReport(FertilizerOutboundSkuGlobalQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<FertilizerOutboundSkuGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetFertilizerOutboundSkuReport", query, request);
        }

        /// <summary>
        /// 渠道库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<ChannelInventoryGlobalDto>> GetChannelInventoryByPage(ChannelInventoryGlobalQuery request)
        {
            var query = new CoreQuery();
            return ApiClient.Post<Pages<ChannelInventoryGlobalDto>>(PublicConst.WmsReportApiUrl, "/Global/GetChannelInventoryByPage", query, request);
        }
    }
}