using System;
using System.Collections.Generic;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Chart;

namespace NBK.ECService.WMSReport.Repository.Interface
{
    public interface IChartRepository : ICrudRepository
    {
        List<AdventSkuChartDto> GetAdventSkuChartDto(DateTime warningDate, Guid wareHouseSysId);

        List<SkuReceiptReportDto> GetSkuReceiptChart(DateTime startDate, DateTime endDate, Guid wareHouseSysId);

        List<SkuOutboundReportDto> GetSkuOutboundChart(DateTime startDate, DateTime endDate, Guid wareHouseSysId);
        /// <summary>
        /// 首页预包装看板
        /// </summary>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        List<PrePackBoardDto> GetPrePackBoardTop12(Guid wareHouseSysId);

        /// <summary>
        /// 超过三天未收货的入库单
        /// </summary>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        List<ExceedThreeDaysPurchase> GetExceedThreeDaysPurchase(Guid wareHouseSysId);

        /// <summary>
        /// 出入库单据
        /// </summary>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        PurchaseAndOutboundChartDto GetPurchaseAndOutboundChart(Guid wareHouseSysId);

        /// <summary>
        /// 近10日订单和退货
        /// </summary>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        List<OutboundAndReturnCharDto> GetOutboundAndReturnCharDataOfLastTenDays(Guid wareHouseSysId);

        List<OutboundChartDto> GetToDayOrderStatusTotal(Guid wareHouseSysId, string startTime, string endTime);

    }
}