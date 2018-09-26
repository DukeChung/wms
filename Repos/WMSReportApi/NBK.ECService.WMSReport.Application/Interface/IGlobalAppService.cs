using Abp.Application.Services;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Other;
using NBK.ECService.WMSReport.DTO.Query;
using NBK.ECService.WMSReport.DTO.Report;
using NBK.ECService.WMSReport.Utility;
using System;
using System.Collections.Generic;

namespace NBK.ECService.WMSReport.Application.Interface
{
    public interface IGlobalAppService : IApplicationService
    {
        List<WareHouseDto> GetAllWarehouse();

        Pages<PurchaseDetailGlobalDto> GetPurchaseDetailReport(PurchaseDetailGlobalQuery request);

        Pages<InvSkuLocGlobalDto> GetInvLocBySkuReport(InvSkuLocGlobalQuery request);

        Pages<InvSkuLotGlobalDto> GetInvLotBySkuReport(InvSkuLotGlobalQuery request);

        Pages<InvSkuLotLocLpnGlobalDto> GetInvLotLocLpnBySkuReport(InvSkuLotLocLpnGlobalQuery request);

        Pages<InvSkuLotGlobalDto> GetExpiryInvLotBySkuReport(InvSkuLotGlobalQuery request);

        Pages<ReceiptDetailGlobalDto> GetReceiptDetailReport(ReceiptDetailGlobalQuery request);

        Pages<OutboundDetailGlobalDto> GetOutboundDetailReport(OutboundDetailGlobalQuery request);

        UploadResultInformation GetOutboundDetailReportByFile(OutboundDetailGlobalQuery request);

        Pages<InvSkuGlobalDto> GetInvSkuReport(InvSkuGlobalQuery request);

        Pages<ReceivedAndSendSkuGlobalDto> GetReceivedAndSendSkuReport(ReceivedAndSendSkuGlobalQuery request);

        Pages<FinanceInvoicingGlobalDto> GetFinanceInvoicingReport(FinanceInvoicingGlobalQueryDto request);

        Pages<AdjustmentDetailGlobalDto> GetAdjustmentDetailReport(AdjustmentDetailGlobalQuery request);

        List<SelectItem> GetSysCodeDetailList(string sysCodeType, bool isActive);

        Pages<InboundGlobalDto> GetInboundReport(InboundGlobalQuery inboundReportQuery);

        Pages<TransferinventoryGlobalDto> GetTransferinventoryReport(TransferinventoryGlobalQuery request);

        Pages<FrozenSkuGlobalDto> GetFrozenSkuReport(FrozenSkuGlobalQuery request);

        Pages<OutboundHandleDateStatisticsGlobalDto> GetOutboundHandleDateStatisticsReport(OutboundHandleDateStatisticsGlobalQuery request);

        Pages<ReceiptAndDeliveryDateGlobalDto> GetReceiptAndDeliveryDateReport(ReceiptAndDeliveryDateGlobalQuery request);

        Pages<SNManageGlobalDto> GetSNManageReport(SNManageGlobalQuery request);

        Pages<OutboundExceptionGlobalDto> GetOutboundExceptionReport(OutboundExceptionGlobalQuery request);

        Pages<OutboundBoxGlobalDto> GetOutboundBoxReport(OutboundBoxGlobalQuery request);

        Pages<BalanceGlobalDto> GetBalanceReport(BalanceGlobalQuery request);

        Pages<OutboundPackGlobalDto> GetOutboundPackReport(OutboundPackGlobalQuery request);

        Pages<OutboundPackSkuGlobalDto> GetOutboundPackSkuReport(OutboundPackSkuGlobalQuery request);

        Pages<SkuPackGlobalListDto> GetSkuPackReport(SkuPackGlobalQuery request);

        Pages<OutboundSkuGlobalDto> GetOutboundSkuReport(OutboundSkuGlobalQuery request);

        Pages<PickingTimeSpanGlobalDto> GetPickingTimeSpanReport(PickingTimeSpanGlobalQuery request);

        Pages<OutboundTransferOrderGlobalDto> GetOutboundTransferOrderReport(OutboundTransferOrderGlobalQuery request);

        List<AccessBizMappingDto> GetAccessBizMappingList(bool flag);

        List<FertilizerRORadarGlobalDto> GetFertilizerRORadarList(bool flag, FertilizerRORadarGlobalQuery request);

        List<FertilizerInvRadarGlobalDto> GetFertilizerInvRadarList(bool flag, FertilizerInvRadarGlobalQuery request);

        List<FertilizerInvPieGlobalDto> GetFertilizerInvPieList(bool flag, FertilizerInvPieGlobalQuery request);
        Pages<ReturnOrderGlobalDto> GetReturnOrderGlobalReport(ReturnOrderGlobalQuery request);

        UploadResultInformation GetReturnOrderReportExport(ReturnOrderGlobalQuery request);

        Pages<OutboundListGlobalDto> GetOutboundSummaryReport(OutboundGlobalQuery request);

        UploadResultInformation GetOutboundSummaryExport(OutboundGlobalQuery request);

        Pages<FertilizerOutboundSkuGlobalDto> GetFertilizerOutboundSkuReport(FertilizerOutboundSkuGlobalQuery request);

        Pages<ChannelInventoryGlobalDto> GetChannelInventoryByPage(ChannelInventoryGlobalQuery request);

    }
}
