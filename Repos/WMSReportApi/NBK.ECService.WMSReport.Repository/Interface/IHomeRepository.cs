using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Chart;
using NBK.ECService.WMSReport.Model.Models;

namespace NBK.ECService.WMSReport.Repository.Interface
{
    public interface IHomeRepository : ICrudRepository
    {
        SparkLineSummaryDto GetSparkLineSummaryDto();

        List<PurchaseTypePieDto> GetPurchaseTypePieDto();

        List<OutboundTypePieDto> GetOutboundTypePieDto();

        List<StockInOutListDto> GetStockInOutData(string startTime, string endTime, ref decimal qty);

        List<WareHouseQtyDto> GetWareHouseReceiptQtyList();

        List<WareHouseQtyDto> GetWareHouseOutboundQtyList();

        List<WareHouseQtyDto> GetWareHouseQtyList();

        StockAgeGroupDto GetStockAgeGroup();

        List<SkuSaleQtyDto> GetSkuSellingTop10();

        List<SkuSaleQtyDto> GetSkuUnsalableTop10();

        List<ServiceStationOutboundDto> GetServiceStationOutboundTopTen();

        List<ChannelQtyDto> GetChannelPieData();

        List<ReturnPurchaseDto> GetReturnPurchase();

        CalendarOrdersOutInData GetCalendarData(string startDate, string endDate);

        CalendarDataByDateOutboundOrPurchase GetCalendarDataByDate(string date);

        List<OutboundLngLatDto> GetNeedToDoLngLatData(int iDisplayStart, int iDisplayLength);

        int GetOutboundNoLngLatCount();

        void UpdateOutboundLngLat(List<OutboundLngLatDto> list);

        List<OutboundMapData> GetHistoryServiceStationData(int page);

        List<WarehouseStationRelationDto> GetWarehouseStationRelation();

        List<WorkDistributionDataDto> GetWorkDistributionData();

        List<WorkDistributionDataByWarehouseDto> GetWorkDistributionByWarehouse(Guid sysId);

        List<WorkDistributionPieData> GetWorkDistributionPieData();
    }
}
