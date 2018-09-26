using Abp.Application.Services;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.Application.Interface
{
    public interface IHomeAppService : IApplicationService
    {

        SparkLineSummaryDto GetSparkLineSummaryDto(bool flag);

        ReturnPurchaseTypePieDto GetPurchaseTypePieDto(bool flag);

        ReturnOutboundTypePieDto GetOutboundTypePieDto(bool flag);

        List<StockInOutListDto> GetStockInOutData(bool flag);

        List<WareHouseQtyDto> GetWareHouseReceiptQtyList(bool flag);

        List<WareHouseQtyDto> GetWareHouseOutboundQtyList(bool flag);

        List<WareHouseQtyDto> GetWareHouseQtyList(bool flag);

        StockAgeGroupDto GetStockAgeGroup(bool flag);

        List<SkuSaleQtyDto> GetSkuSellingTop10(bool flag);

        List<SkuSaleQtyDto> GetSkuUnsalableTop10(bool flag);

        /// <summary>
        /// 获取服务站发货Top10
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        List<ServiceStationOutboundDto> GetServiceStationOutboundTopTen(bool flag);

        List<ChannelQtyDto> GetChannelPieData(bool flag);

        List<ReturnPurchaseDto> GetReturnPurchase(bool flag);

        int GetOutboundNoLngLatCount();

        bool UpdataOutboundLngLat(int page);

        List<OutboundMapData> GetHistoryServiceStationData(int page);

        List<WarehouseStationRelationDto> GetWarehouseStationRelation();

        EventDataCountReportDto GetDailyEventDataCountInfo(bool flag, string startDate, string endDate);

        List<CalendarDataByDateDto> GetCalendarDataByDate(string date);

        List<WorkDistributionListData> GetWorkDistributionData(bool flag);

        List<WorkDistributionDataByWarehouseDto> GetWorkDistributionByWarehouse(bool flag, Guid sysId);

        List<WorkDistributionPieDto> GetWorkDistributionPieData(bool flag);
    }
}
