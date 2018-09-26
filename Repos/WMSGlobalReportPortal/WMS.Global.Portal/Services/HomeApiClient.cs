using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Chart;
using NBK.ECService.WMSReport.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS.Global.Portal.Services
{
    public class HomeApiClient
    {
        private static readonly HomeApiClient instance = new HomeApiClient();

        private HomeApiClient() { }

        public static HomeApiClient GetInstance()
        {
            return instance;
        }

        public ApiResponse<SparkLineSummaryDto> GetSparkLineSummaryDto()
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false };
            return ApiClient.Get<SparkLineSummaryDto>(PublicConst.WmsReportApiUrl, "/Home/GetSparkLineSummaryDto", query);
        }

        public ApiResponse<ReturnPurchaseTypePieDto> GetPurchaseTypePieDto()
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false };
            return ApiClient.Get<ReturnPurchaseTypePieDto>(PublicConst.WmsReportApiUrl, "/Home/GetPurchaseTypePieDto", query);
        }

        public ApiResponse<ReturnOutboundTypePieDto> GetOutboundTypePieDto()
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false };
            return ApiClient.Get<ReturnOutboundTypePieDto>(PublicConst.WmsReportApiUrl, "/Home/GetOutboundTypePieDto", query);
        }

        public ApiResponse<List<StockInOutListDto>> GetStockInOutData()
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false };
            return ApiClient.Get<List<StockInOutListDto>>(PublicConst.WmsReportApiUrl, "/Home/GetStockInOutData", query);
        }

        public ApiResponse<List<WareHouseQtyDto>> GetWareHouseReceiptQtyList()
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false };
            return ApiClient.Get<List<WareHouseQtyDto>>(PublicConst.WmsReportApiUrl, "/Home/GetWareHouseReceiptQtyList", query);
        }

        public ApiResponse<List<WareHouseQtyDto>> GetWareHouseOutboundQtyList()
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false };
            return ApiClient.Get<List<WareHouseQtyDto>>(PublicConst.WmsReportApiUrl, "/Home/GetWareHouseOutboundQtyList", query);
        }

        public ApiResponse<List<WareHouseQtyDto>> GetWareHouseQtyList()
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false };
            return ApiClient.Get<List<WareHouseQtyDto>>(PublicConst.WmsReportApiUrl, "/Home/GetWareHouseQtyList", query);
        }
        public ApiResponse<StockAgeGroupDto> GetStockAgeGroup()
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false };
            return ApiClient.Get<StockAgeGroupDto>(PublicConst.WmsReportApiUrl, "/Home/GetStockAgeGroup", query);
        }

        public ApiResponse<List<SkuSaleQtyDto>> GetSkuSellingTop10()
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false };
            return ApiClient.Get<List<SkuSaleQtyDto>>(PublicConst.WmsReportApiUrl, "/Home/GetSkuSellingTop10", query);
        }

        public ApiResponse<List<SkuSaleQtyDto>> GetSkuUnsalableTop10()
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false };
            return ApiClient.Get<List<SkuSaleQtyDto>>(PublicConst.WmsReportApiUrl, "/Home/GetSkuUnsalableTop10", query);
        }

        public ApiResponse<List<ChannelQtyDto>> GetChannelPieData()
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false };
            return ApiClient.Get<List<ChannelQtyDto>>(PublicConst.WmsReportApiUrl, "/Home/GetChannelPieData", query);
        }

        /// <summary>
        /// 获取服务站发货Top10
        /// </summary>
        /// <returns></returns>
        public ApiResponse<List<ServiceStationOutboundDto>> GetServiceStationOutboundTopTen()
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false };
            return ApiClient.Get<List<ServiceStationOutboundDto>>(PublicConst.WmsReportApiUrl, "/Home/GetServiceStationOutboundTopTen", query);
        }

        public ApiResponse<List<ReturnPurchaseDto>> GetReturnPurchase()
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false };
            return ApiClient.Get<List<ReturnPurchaseDto>>(PublicConst.WmsReportApiUrl, "/Home/GetReturnPurchase", query);
        }

        /// <summary>
        /// 获取仓库业务分部
        /// </summary>
        /// <returns></returns>
        public ApiResponse<List<OutboundMapData>> GetHistoryServiceStationData(int page)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { page = page };
            return ApiClient.Get<List<OutboundMapData>>(PublicConst.WmsReportApiUrl, "/Home/GetHistoryServiceStationData", query);
        }

        /// <summary>
        /// 获取仓库业务分部
        /// </summary>
        /// <returns></returns>
        public ApiResponse<List<WarehouseStationRelationDto>> GetWarehouseStationRelation()
        {
            var query = new CoreQuery();
            return ApiClient.Get<List<WarehouseStationRelationDto>>(PublicConst.WmsReportApiUrl, "/Home/GetWarehouseStationRelation", query);
        }

        public ApiResponse<EventDataCountReportDto> GetDailyEventDataCountInfo(string start, string end)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false, startDate = start, endDate = end };
            return ApiClient.Get<EventDataCountReportDto>(PublicConst.WmsReportApiUrl, "/Home/GetDailyEventDataCountInfo", query);
        }

        public ApiResponse<List<AccessBizMappingDto>> GetAccessBizList()
        {
            return ApiClient.Get<List<AccessBizMappingDto>>(PublicConst.WmsReportApiUrl, "/Global/GetAccessBizList", new CoreQuery() { ParmsObj = new { flag = false } });
        }


        public ApiResponse<List<CalendarDataByDateDto>> GetCalendarDataByDate(string date)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { date = date };
            return ApiClient.Get<List<CalendarDataByDateDto>>(PublicConst.WmsReportApiUrl, "/Home/GetCalendarDataByDate", query);
        }

        /// <summary>
        /// 获取所有仓库作业时间分布
        /// </summary>
        /// <returns></returns>
        public ApiResponse<List<WorkDistributionListData>> GetWorkDistributionData()
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false, };
            return ApiClient.Get<List<WorkDistributionListData>>(PublicConst.WmsReportApiUrl, "/Home/GetWorkDistributionData", query);
        }

        /// <summary>
        /// 获取所有仓库作业时间分布
        /// </summary>
        /// <returns></returns>
        public ApiResponse<List<WorkDistributionDataByWarehouseDto>> GetWorkDistributionByWarehouse(Guid sysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false, sysId = sysId };
            return ApiClient.Get<List<WorkDistributionDataByWarehouseDto>>(PublicConst.WmsReportApiUrl, "/Home/GetWorkDistributionByWarehouse", query);
        }

        /// <summary>
        /// 仓库作业占比
        /// </summary>
        /// <returns></returns>
        public ApiResponse<List<WorkDistributionPieDto>> GetWorkDistributionPieData()
        {
            var query = new CoreQuery();
            query.ParmsObj = new { flag = false, };
            return ApiClient.Get<List<WorkDistributionPieDto>>(PublicConst.WmsReportApiUrl, "/Home/GetWorkDistributionPieData", query);
        }
    }
}