using System;
using System.Collections.Generic;
using Abp.Application.Services;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Chart;

namespace NBK.ECService.WMSReport.Application.Interface
{
    public interface IChartAppService : IApplicationService
    {
        /// <summary>
        /// 货卡查询
        /// </summary>
        /// <param name="invTransBySkuReportQuery"></param>
        /// <returns></returns>
        PurchaseAndOutboundChartDto GetPurchaseAndOutboundChart(Guid wareHouseSysId);

        /// <summary>
        /// 获取临期商品
        /// </summary>
        /// <returns></returns>
        List<AdventSkuChartDto> GetAdventSkuChartTop5(Guid wareHouseSysId);

        List<SkuReceiptOutboundReportDto> GetSkuReceiptOutboundChartAfter10(Guid wareHouseSysId);

        /// <summary>
        /// 获取过去十天订单与退货数量
        /// </summary>
        /// <returns></returns>
        List<OutboundAndReturnCharDto> GetOutboundAndReturnCharDataOfLastTenDays(Guid wareHouseSysId);

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
        /// 清楚首页报表缓存
        /// </summary>
        void CleanReceiptOutboundRedis(Guid wareHouseSysId);

        /// <summary>
        /// 获取当天订单状态总数
        /// </summary>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        OutboundTotalChartDto GetToDayOrderStatusTotal(Guid wareHouseSysId);
    }
}