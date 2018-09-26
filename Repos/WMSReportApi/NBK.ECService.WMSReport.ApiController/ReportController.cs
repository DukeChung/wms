using System.Web.Http;
using Abp.WebApi.Controllers;
using NBK.ECService.WMSReport.Application.Interface;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Base;
using System;
using System.Collections.Generic;
using NBK.ECService.WMSReport.DTO.Report;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.DTO.Query;
using NBK.ECService.WMSReport.DTO.Other;

namespace NBK.ECService.WMSReport.ApiController
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/Report")]
    public class ReportController : AbpApiController
    {
        private IReportAppService _reportAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportAppService"></param>
        public ReportController(IReportAppService reportAppService)
        {
            _reportAppService = reportAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void PrintAPI() { }

        /// <summary>
        /// 货卡查询
        /// </summary>
        /// <param name="invTransBySkuReportQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetInvTransBySkuReport")]
        public InvTransSkuListReportDto GetInvTransBySkuReport(InvTransBySkuReportQuery invTransBySkuReportQuery)
        {
            return _reportAppService.GetInvTransBySkuListReport(invTransBySkuReportQuery);
        }

        /// <summary>
        /// 货位库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetInvLocBySkuReport")]
        public Pages<InvSkuLocReportDto> GetInvLocBySkuReport(InvSkuLocReportQuery request)
        {
            return _reportAppService.GetInvLocBySkuReport(request);
        }

        /// <summary>
        /// 批次库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetInvLotBySkuReport")]
        public Pages<InvSkuLotReportDto> GetInvLotBySkuReport(InvSkuLotReportQuery request)
        {
            return _reportAppService.GetInvLotBySkuReport(request);
        }

        /// <summary>
        /// 货位批次库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetInvLotLocLpnBySkuReport")]
        public Pages<InvSkuLotLocLpnReportDto> GetInvLotLocLpnBySkuReport(InvSkuLotLocLpnReportQuery request)
        {
            return _reportAppService.GetInvLotLocLpnBySkuReport(request);
        }

        /// <summary>
        /// 获取临过期批次商品库存信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetExpiryInvLotBySkuReport")]
        public Pages<InvSkuLotReportDto> GetExpiryInvLotBySkuReport(InvSkuLotReportQuery request)
        {
            return _reportAppService.GetExpiryInvLotBySkuReport(request);
        }

        /// <summary>
        /// 收货明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReceiptDetailReport")]
        public Pages<ReceiptDetailReportDto> GetReceiptDetailReport(ReceiptDetailReportQuery request)
        {
            return _reportAppService.GetReceiptDetailReport(request);
        }

        /// <summary>
        /// 出库明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundDetailReport")]
        public Pages<OutboundDetailReportDto> GetOutboundDetailReport(OutboundDetailReportQuery request)
        {
            return _reportAppService.GetOutboundDetailReport(request);
        }

        /// <summary>
        /// 出库明细报表上传
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundDetailReportByFile")]
        public UploadResultInformation GetOutboundDetailReportByFile(OutboundDetailReportQuery request)
        {
            return _reportAppService.GetOutboundDetailReportByFile(request);
        }

        /// <summary>
        /// 库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetInvSkuReport")]
        public Pages<InvSkuReportDto> GetInvSkuReport(InvSkuReportQuery request)
        {
            return _reportAppService.GetInvSkuReport(request);
        }

        /// <summary>
        /// 进销存查询
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetFinanceInvoicingReport")]
        public Pages<FinanceInvoicingReportDto> GetFinanceInvoicingReport(FinanceInvoicingReportQueryDto request)
        {
            return _reportAppService.GetFinanceInvoicingReport(request);
        }

        /// <summary>
        /// 损益明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetAdjustmentDetailReport")]
        public Pages<AdjustmentDetailReportDto> GetAdjustmentDetailReport(AdjustmentDetailReportQuery request)
        {
            return _reportAppService.GetAdjustmentDetailReport(request);
        }

        /// <summary>
        /// 入库明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPurchaseDetailReport")]
        public Pages<PurchaseDetailReportDto> GetPurchaseDetailReport(PurchaseDetailReportQuery request)
        {
            return _reportAppService.GetPurchaseDetailReport(request);
        }


        /// <summary>
        /// 入库汇总查询
        /// </summary>
        /// <param name="inboundReportQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetInboundReport")]
        public Pages<InboundReportDto> GetInboundReport(InboundReportQuery inboundReportQuery)
        {
            return _reportAppService.GetInboundReport(inboundReportQuery);
        }

        /// <summary>
        /// 分页查询商品外借
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetSkuBorrowListByPage")]
        public Pages<SkuBorrowReportDto> GetSkuBorrowListByPage(SkuBorrowReportQuery request)
        {
            return _reportAppService.GetSkuBorrowListByPage(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetFrozenSkuReport")]
        public Pages<FrozenSkuReportDto> GetFrozenSkuReport(FrozenSkuReportQuery request)
        {
            return _reportAppService.GetFrozenSkuReport(request);
        }

        /// <summary>
        /// 冻结商品明细导出
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetFrozenSkuReportByFile")]
        public UploadResultInformation GetFrozenSkuReportByFile(FrozenSkuReportQuery request)
        {
            return _reportAppService.GetFrozenSkuReportByFile(request);
        }

        /// <summary>
        /// 仓库收发货明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReceivedAndSendSkuReport")]
        public Pages<ReceivedAndSendSkuReportDto> GetReceivedAndSendSkuReport(ReceivedAndSendSkuReportQuery request)
        {
            return _reportAppService.GetReceivedAndSendSkuReport(request);
        }


        /// <summary>
        /// 出库处理时间统计表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundHandleDateStatisticsReport")]
        public Pages<OutboundHandleDateStatisticsDto> GetOutboundHandleDateStatisticsReport(OutboundHandleDateStatisticsQuery request)
        {
            return _reportAppService.GetOutboundHandleDateStatisticsReport(request);
        }

        /// <summary>
        /// 出库处理时间统计表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReceiptAndDeliveryDateReport")]
        public Pages<ReceiptAndDeliveryDateDto> GetReceiptAndDeliveryDateReport(ReceiptAndDeliveryDateQuery request)
        {
            return _reportAppService.GetReceiptAndDeliveryDateReport(request);
        }

        /// <summary>
        /// 监控报表-入库
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetMonitorReportPurchaseDto")]
        public Pages<PurchaseMonitorDto> GetMonitorReportPurchaseDto(MonitorReportQuery request)
        {
            return _reportAppService.GetMonitorReportPurchaseDto(request);
        }

        /// <summary>
        /// 监控报表-出库
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetMonitorReportOutBoundDto")]
        public Pages<OutboundMonitorDto> GetMonitorReportOutBoundDto(MonitorReportQuery request)
        {
            return _reportAppService.GetMonitorReportOutBoundDto(request);
        }

        /// <summary>
        /// 监控报表-工单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetMonitorReportWorkDto")]
        public Pages<WorkMonitorDto> GetMonitorReportWorkDto(MonitorReportQuery request)
        {
            return _reportAppService.GetMonitorReportWorkDto(request);
        }

        /// <summary>
        /// 监控报表-退货入库
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetMonitorReportPurchaseReturnDto")]
        public Pages<PurchaseReturnMonitorDto> GetMonitorReportPurchaseReturnDto(MonitorReportQuery request)
        {
            return _reportAppService.GetMonitorReportPurchaseReturnDto(request);
        }

        /// <summary>
        /// 监控报表-库存转移
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetMonitorReportStockTransferDto")]
        public Pages<StockTransferMonitorDto> GetMonitorReportStockTransferDto(MonitorReportQuery request)
        {
            return _reportAppService.GetMonitorReportStockTransferDto(request);
        }

        /// <summary>
        /// SN报表查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetSNManageReport")]
        public Pages<SNManageReportDto> GetSNManageReport(SNManageReportQuery request)
        {
            return _reportAppService.GetSNManageReport(request);
        }


        /// <summary>
        /// 异常报告报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundExceptionReport")]
        public Pages<OutboundExceptionReportDto> GetOutboundExceptionReport(OutboundExceptionReportQuery request)
        {
            return _reportAppService.GetOutboundExceptionReport(request);
        }

        /// <summary>
        /// 出库箱数统计报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundBoxReport")]
        public Pages<OutboundBoxReportDto> GetOutboundBoxReport(OutboundBoxReportQuery request)
        {
            return _reportAppService.GetOutboundBoxReport(request);
        }

        /// <summary>
        /// B2C结算报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetBalanceReport")]
        public Pages<BalanceReportDto> GetBalanceReport(BalanceReportQuery request)
        {
            return _reportAppService.GetBalanceReport(request);
        }

        /// <summary>
        /// 仓储费结算统计明表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundPackReport")]
        public Pages<OutboundPackReportDto> GetOutboundPackReport(OutboundPackReportQuery request)
        {
            return _reportAppService.GetOutboundPackReport(request);
        }

        /// <summary>
        /// 整散箱商品明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundPackSkuReport")]
        public Pages<OutboundPackSkuReportDto> GetOutboundPackSkuReport(OutboundPackSkuReportQuery request)
        {
            return _reportAppService.GetOutboundPackSkuReport(request);
        }

        /// <summary>
        /// 商品包装查询报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetSkuPackReport")]
        public Pages<SkuPackReportListDto> GetSkuPackReport(SkuPackReportQuery request)
        {
            return _reportAppService.GetSkuPackReport(request);
        }

        /// <summary>
        /// 出库汇总报表查看
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundSummaryReport")]
        public Pages<OutboundListDto> GetOutboundSummaryReport(OutboundQuery request)
        {
            return _reportAppService.GetOutboundSummaryReport(request);
        }


        /// <summary>
        /// 出库汇总报表导出
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundSummaryExport")]
        public UploadResultInformation GetOutboundSummaryExport(OutboundQuery request)
        {
            return _reportAppService.GetOutboundSummaryExport(request);
        }

        /// <summary>
        /// 渠道库存报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetChannelInventoryByPage")]
        public Pages<ChannelInventoryDto> GetChannelInventoryByPage(ChannelInventoryQuery request)
        {
            return _reportAppService.GetChannelInventoryByPage(request);
        }
    }
}