using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Chart;
using NBK.ECService.WMSReport.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS.Report.Portal.Services
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
            return ApiClient.Get<SparkLineSummaryDto>(PublicConst.WmsReportApiUrl, "/Home/GetSparkLineSummaryDto", new CoreQuery());
        }

        //public ApiResponse<ProgressBarSummaryDto> GetProgressBarSummaryDto()
        //{
        //    return ApiClient.Get<ProgressBarSummaryDto>(PublicConst.WmsReportApiUrl, "/Home/GetProgressBarSummaryDto", new CoreQuery());
        //}

        //public ApiResponse<PieChartSummaryDto> GetPieChartSummaryDto()
        //{
        //    return ApiClient.Get<PieChartSummaryDto>(PublicConst.WmsReportApiUrl, "/Home/GetPieChartSummaryDto", new CoreQuery());
        //}

        //public ApiResponse<FlotChartSummaryDto> GetFlotChartSummaryDto()
        //{
        //    return ApiClient.Get<FlotChartSummaryDto>(PublicConst.WmsReportApiUrl, "/Home/GetFlotChartSummaryDto", new CoreQuery());
        //}

        //public ApiResponse<EventDataCountReportDto> GetDailyEventDataCountInfo()
        //{
        //    return ApiClient.Get<EventDataCountReportDto>(PublicConst.WmsReportApiUrl, "/Home/GetDailyEventDataCountInfo", new CoreQuery());
        //}

        //public ApiResponse<ApiProcessResultTotalDto> GetApiProcessResult()
        //{
        //    var query = new CoreQuery();
        //    query.ParmsObj = new { secondInterval = 3, systemId = 10 };
        //    return ApiClient.Get<ApiProcessResultTotalDto>(PublicConst.WmsLogApiUrl, "/BusinessLog/GetApiProcessResultBySecondInterval", query);
        //}

        //public ApiResponse<Pages<ServiceStationReceiptDto>> GetServiceStationReceiptInfo(ServiceStationReceiptQuery request)
        //{
        //    var query = new CoreQuery();
        //    return ApiClient.Post<Pages<ServiceStationReceiptDto>>(PublicConst.WmsReportApiUrl, "/Home/GetServiceStationReceiptInfo", query, request);
        //}

        //public ApiResponse<Pages<TurnoverSkuDto>> GetTurnoverSkuPage(BaseQuery baseQuery)
        //{
        //    var query = new CoreQuery();
        //    return ApiClient.Post<Pages<TurnoverSkuDto>>(PublicConst.WmsReportApiUrl, "/Home/GetTurnoverSkuByPage", query, baseQuery);
        //}

        //public ApiResponse<string> GetBirdsEysSource()
        //{
        //    return ApiClient.Post<string>(PublicConst.WmsReportApiUrl, "/Home/GetBirdsEysSource", new CoreQuery());
        //}

        //public ApiResponse<OutboundMapData> GetCurrentOutbound()
        //{
        //    return ApiClient.Post<OutboundMapData>(PublicConst.WmsReportApiUrl, "/Home/GetCurrentOutbound", new CoreQuery());
        //}

        //public ApiResponse<List<BirdsEysSourceDto>> GetNewOutbound()
        //{
        //    return ApiClient.Post<List<BirdsEysSourceDto>>(PublicConst.WmsReportApiUrl, "/Home/GetNewOutbound", new CoreQuery());
        //}

        ///// <summary>
        ///// 库存商品涨跌图表
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //public ApiResponse<Pages<InvSkuChangeReportDto>> GetInvSkuChangeReport(InvSkuChangeReportQuery request)
        //{
        //    return ApiClient.Post<Pages<InvSkuChangeReportDto>>(PublicConst.WmsReportApiUrl, "/Home/GetInvSkuChangeReport", new CoreQuery(), request);
        //}

        ///// <summary>
        ///// 商品发货量最大排名
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //public ApiResponse<SkuShipmentsRankDto> GetMaxSkuShipmentsRank(BaseQuery request)
        //{
        //    return ApiClient.Post<SkuShipmentsRankDto>(PublicConst.WmsReportApiUrl, "/Home/GetMaxSkuShipmentsRank", new CoreQuery(), request);
        //}

        ///// <summary>
        ///// 商品发货量最小排名
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //public ApiResponse<SkuShipmentsRankDto> GetMinSkuShipmentsRank(BaseQuery request)
        //{
        //    return ApiClient.Post<SkuShipmentsRankDto>(PublicConst.WmsReportApiUrl, "/Home/GetMinSkuShipmentsRank", new CoreQuery(), request);
        //}

        ///// <summary>
        ///// 获取10天仓库入库数据
        ///// </summary>
        ///// <returns></returns>
        //public ApiResponse<WareHouseReceiptOutboundDto> GetReceiptListData()
        //{
        //    return ApiClient.Get<WareHouseReceiptOutboundDto>(PublicConst.WmsReportApiUrl, "/Home/GetReceiptListData", new CoreQuery());
        //}


        ///// <summary>
        ///// 获取10天仓库出库数据
        ///// </summary>
        ///// <returns></returns>
        //public ApiResponse<WareHouseReceiptOutboundDto> GetOutboundListData()
        //{
        //    return ApiClient.Get<WareHouseReceiptOutboundDto>(PublicConst.WmsReportApiUrl, "/Home/GetOutboundListData", new CoreQuery());
        //}

        ///// <summary>
        ///// 各渠道库存占比
        ///// </summary>
        ///// <param name="whflag"></param>
        ///// <returns></returns>
        //public ApiResponse<List<ChannelInventoryDto>> GetChannelInventoryData(int whflag)
        //{
        //    var query = new CoreQuery();
        //    query.ParmsObj = new { whflag = whflag };
        //    return ApiClient.Get<List<ChannelInventoryDto>>(PublicConst.WmsReportApiUrl, "/Home/GetChannelInventoryData", query);
        //}

        ///// <summary>
        ///// 各渠道库存占比
        ///// </summary>
        ///// <param name="whflag"></param>
        ///// <returns></returns>
        //public ApiResponse<EachChannelInvOutChartDto> GetEachChannelInvOut(ChannelInvOutChartQueryDto dto)
        //{
        //    var query = new CoreQuery();
        //    return ApiClient.Post<EachChannelInvOutChartDto>(PublicConst.WmsReportApiUrl, "/Home/GetEachChannelInvOut", query, dto);
        //}

        ///// <summary>
        ///// 各渠道库存占比
        ///// </summary>
        ///// <param name="whflag"></param>
        ///// <returns></returns>
        //public ApiResponse<InventoryTypeDistribDto> GetInventoryTypeDistrib(int flag)
        //{
        //    var query = new CoreQuery();
        //    query.ParmsObj = new { flag = flag };
        //    return ApiClient.Post<InventoryTypeDistribDto>(PublicConst.WmsReportApiUrl, "/Home/GetInventoryTypeDistrib", query);
        //}

        //public ApiResponse<WorkDistributionDto> GetWorkDistribution(int flag)
        //{
        //    var query = new CoreQuery();
        //    query.ParmsObj = new { flag = flag };
        //    return ApiClient.Post<WorkDistributionDto>(PublicConst.WmsReportApiUrl, "/Home/GetWorkDistribution", query);
        //}
    }
}