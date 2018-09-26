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
    [RoutePrefix("api/Global")]
    public class GlobalController : AbpApiController
    {
        private IGlobalAppService _globalAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="globalAppService"></param>
        public GlobalController(IGlobalAppService globalAppService)
        {
            _globalAppService = globalAppService;
        }

        /// <summary>
        /// 获取所有仓库
        /// </summary> 
        /// <returns></returns>
        [HttpPost, Route("GetAllWarehouse")]
        public List<WareHouseDto> GetAllWarehouse()
        {
            return _globalAppService.GetAllWarehouse();
        }

        /// <summary>
        /// 入库明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPurchaseDetailReport")]
        public Pages<PurchaseDetailGlobalDto> GetPurchaseDetailReport(PurchaseDetailGlobalQuery request)
        {
            return _globalAppService.GetPurchaseDetailReport(request);
        }

        /// <summary>
        /// 货位库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetInvLocBySkuReport")]
        public Pages<InvSkuLocGlobalDto> GetInvLocBySkuReport(InvSkuLocGlobalQuery request)
        {
            return _globalAppService.GetInvLocBySkuReport(request);
        }

        /// <summary>
        /// 批次库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetInvLotBySkuReport")]
        public Pages<InvSkuLotGlobalDto> GetInvLotBySkuReport(InvSkuLotGlobalQuery request)
        {
            return _globalAppService.GetInvLotBySkuReport(request);
        }

        /// <summary>
        /// 货位批次库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetInvLotLocLpnBySkuReport")]
        public Pages<InvSkuLotLocLpnGlobalDto> GetInvLotLocLpnBySkuReport(InvSkuLotLocLpnGlobalQuery request)
        {
            return _globalAppService.GetInvLotLocLpnBySkuReport(request);
        }

        /// <summary>
        /// 获取临过期批次商品库存信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetExpiryInvLotBySkuReport")]
        public Pages<InvSkuLotGlobalDto> GetExpiryInvLotBySkuReport(InvSkuLotGlobalQuery request)
        {
            return _globalAppService.GetExpiryInvLotBySkuReport(request);
        }

        /// <summary>
        /// 收货明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReceiptDetailReport")]
        public Pages<ReceiptDetailGlobalDto> GetReceiptDetailReport(ReceiptDetailGlobalQuery request)
        {
            return _globalAppService.GetReceiptDetailReport(request);
        }

        /// <summary>
        /// 出库明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundDetailReport")]
        public Pages<OutboundDetailGlobalDto> GetOutboundDetailReport(OutboundDetailGlobalQuery request)
        {
            return _globalAppService.GetOutboundDetailReport(request);
        }


        /// <summary>
        /// 出库明细报表导出
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundDetailReportByFile")]
        public UploadResultInformation GetOutboundDetailReportByFile(OutboundDetailGlobalQuery request)
        {
            return _globalAppService.GetOutboundDetailReportByFile(request);
        }

        /// <summary>
        /// 移仓单报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetTransferinventoryReport")]
        public Pages<TransferinventoryGlobalDto> GetTransferinventoryReport(TransferinventoryGlobalQuery request)
        {
            return _globalAppService.GetTransferinventoryReport(request);
        }

        /// <summary>
        /// 库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetInvSkuReport")]
        public Pages<InvSkuGlobalDto> GetInvSkuReport(InvSkuGlobalQuery request)
        {
            return _globalAppService.GetInvSkuReport(request);
        }

        /// <summary>
        /// 仓库收发货明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReceivedAndSendSkuReport")]
        public Pages<ReceivedAndSendSkuGlobalDto> GetReceivedAndSendSkuReport(ReceivedAndSendSkuGlobalQuery request)
        {
            return _globalAppService.GetReceivedAndSendSkuReport(request);
        }

        /// <summary>
        /// 进销存查询
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetFinanceInvoicingReport")]
        public Pages<FinanceInvoicingGlobalDto> GetFinanceInvoicingReport(FinanceInvoicingGlobalQueryDto request)
        {
            return _globalAppService.GetFinanceInvoicingReport(request);
        }

        /// <summary>
        /// 入库汇总查询
        /// </summary>
        /// <param name="inboundReportQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetInboundReport")]
        public Pages<InboundGlobalDto> GetInboundReport(InboundGlobalQuery inboundReportQuery)
        {
            return _globalAppService.GetInboundReport(inboundReportQuery);
        }


        /// <summary>
        /// 损益明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetAdjustmentDetailReport")]
        public Pages<AdjustmentDetailGlobalDto> GetAdjustmentDetailReport(AdjustmentDetailGlobalQuery request)
        {
            return _globalAppService.GetAdjustmentDetailReport(request);
        }

        /// <summary>
        /// 获取系统设置明细
        /// </summary>
        /// <param name="sysCodeType"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        [HttpGet, Route("GetSelectBySysCode")]
        public List<SelectItem> GetSelectBySysCode(string sysCodeType, bool isActive)
        {
            return _globalAppService.GetSysCodeDetailList(sysCodeType, isActive);
        }

        /// <summary>
        /// 冻结商品明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetFrozenSkuReport")]
        public Pages<FrozenSkuGlobalDto> GetFrozenSkuReport(FrozenSkuGlobalQuery request)
        {
            return _globalAppService.GetFrozenSkuReport(request);
        }

        /// <summary>
        /// 出库处理时间统计表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundHandleDateStatisticsReport")]
        public Pages<OutboundHandleDateStatisticsGlobalDto> GetOutboundHandleDateStatisticsReport(OutboundHandleDateStatisticsGlobalQuery request)
        {
            return _globalAppService.GetOutboundHandleDateStatisticsReport(request);
        }

        /// <summary>
        /// 出库处理时间统计表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReceiptAndDeliveryDateReport")]
        public Pages<ReceiptAndDeliveryDateGlobalDto> GetReceiptAndDeliveryDateReport(ReceiptAndDeliveryDateGlobalQuery request)
        {
            return _globalAppService.GetReceiptAndDeliveryDateReport(request);
        }

        /// <summary>
        /// SN报表查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetSNManageReport")]
        public Pages<SNManageGlobalDto> GetSNManageReport(SNManageGlobalQuery request)
        {
            return _globalAppService.GetSNManageReport(request);
        }

        /// <summary>
        /// 异常报告报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundExceptionReport")]
        public Pages<OutboundExceptionGlobalDto> GetOutboundExceptionReport(OutboundExceptionGlobalQuery request)
        {
            return _globalAppService.GetOutboundExceptionReport(request);
        }

        /// <summary>
        /// 出库箱数统计报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundBoxReport")]
        public Pages<OutboundBoxGlobalDto> GetOutboundBoxReport(OutboundBoxGlobalQuery request)
        {
            return _globalAppService.GetOutboundBoxReport(request);
        }

        /// <summary>
        /// B2C结算报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetBalanceReport")]
        public Pages<BalanceGlobalDto> GetBalanceReport(BalanceGlobalQuery request)
        {
            return _globalAppService.GetBalanceReport(request);
        }

        /// <summary>
        /// 整散箱装箱明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundPackReport")]
        public Pages<OutboundPackGlobalDto> GetOutboundPackReport(OutboundPackGlobalQuery request)
        {
            return _globalAppService.GetOutboundPackReport(request);
        }

        /// <summary>
        /// 整散箱商品明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundPackSkuReport")]
        public Pages<OutboundPackSkuGlobalDto> GetOutboundPackSkuReport(OutboundPackSkuGlobalQuery request)
        {
            return _globalAppService.GetOutboundPackSkuReport(request);
        }

        /// <summary>
        /// 商品包装查询报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetSkuPackReport")]
        public Pages<SkuPackGlobalListDto> GetSkuPackReport(SkuPackGlobalQuery request)
        {
            return _globalAppService.GetSkuPackReport(request);
        }


        /// <summary>
        /// 出库单商品汇总报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundSkuReport")]
        public Pages<OutboundSkuGlobalDto> GetOutboundSkuReport(OutboundSkuGlobalQuery request)
        {
            return _globalAppService.GetOutboundSkuReport(request);
        }


        /// <summary>
        /// 出库捡货工时统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPickingTimeSpanReport")]
        public Pages<PickingTimeSpanGlobalDto> GetPickingTimeSpanReport(PickingTimeSpanGlobalQuery request)
        {
            return _globalAppService.GetPickingTimeSpanReport(request);
        }

        /// <summary>
        /// 出库复核工时统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundTransferOrderReport")]
        public Pages<OutboundTransferOrderGlobalDto> GetOutboundTransferOrderReport(OutboundTransferOrderGlobalQuery request)
        {
            return _globalAppService.GetOutboundTransferOrderReport(request);
        }

        /// <summary>
        /// 业务接口访问量统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetAccessBizList")]
        public List<AccessBizMappingDto> GetAccessBizList(bool flag)
        {
            return _globalAppService.GetAccessBizMappingList(flag);
        }

        /// <summary>
        /// 化肥出入库
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetFertilizerRORadarList")]
        public List<FertilizerRORadarGlobalDto> GetFertilizerRORadarList(bool flag, FertilizerRORadarGlobalQuery request)
        {
            return _globalAppService.GetFertilizerRORadarList(flag, request);
        }

        /// <summary>
        /// 化肥库存
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetFertilizerInvRadarList")]
        public List<FertilizerInvRadarGlobalDto> GetFertilizerInvRadarList(bool flag, FertilizerInvRadarGlobalQuery request)
        {
            return _globalAppService.GetFertilizerInvRadarList(flag, request);
        }

        /// <summary>
        /// 化肥库存分布
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetFertilizerInvPieList")]
        public List<FertilizerInvPieGlobalDto> GetFertilizerInvPieList(bool flag, FertilizerInvPieGlobalQuery request)
        {
            return _globalAppService.GetFertilizerInvPieList(flag, request);
        }

        /// <summary>
        /// 退货单全局报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReturnOrderGlobalReport")]
        public Pages<ReturnOrderGlobalDto> GetReturnOrderGlobalReport(ReturnOrderGlobalQuery request)
        {
            return _globalAppService.GetReturnOrderGlobalReport(request);
        }

        /// <summary>
        /// 退货单全局报表导出
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReturnOrderReportExport")]
        public UploadResultInformation GetReturnOrderReportExport(ReturnOrderGlobalQuery request)
        {
            return _globalAppService.GetReturnOrderReportExport(request);
        }

        /// <summary>
        /// 出库汇总报表查看
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundSummaryReport")]
        public Pages<OutboundListGlobalDto> GetOutboundSummaryReport(OutboundGlobalQuery request)
        {
            return _globalAppService.GetOutboundSummaryReport(request);
        }

        /// <summary>
        /// 出库汇总报表导出
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundSummaryExport")]
        public UploadResultInformation GetOutboundSummaryExport(OutboundGlobalQuery request)
        {
            return _globalAppService.GetOutboundSummaryExport(request);
        }

        /// <summary>
        /// 农资出库商品报表
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetFertilizerOutboundSkuReport")]
        public Pages<FertilizerOutboundSkuGlobalDto> GetFertilizerOutboundSkuReport(FertilizerOutboundSkuGlobalQuery request)
        {
            return _globalAppService.GetFertilizerOutboundSkuReport(request);
        }

        /// <summary>
        /// 渠道库存查询
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetChannelInventoryByPage")]
        public Pages<ChannelInventoryGlobalDto> GetChannelInventoryByPage(ChannelInventoryGlobalQuery request)
        {
            return _globalAppService.GetChannelInventoryByPage(request);
        }
    }
}
