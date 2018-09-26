using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBK.WMSLog.Portal.Services
{
    public class LogApiClient
    {
        private static readonly LogApiClient instance = new LogApiClient();

        private LogApiClient() { }

        public static LogApiClient GetInstance() { return instance; }


        public ApiResponse<InboundBizLogDto> GetInboundBizLogByDays(CoreQuery query, int days)
        {
            query.ParmsObj = new { days };
            return ApiClient.Get<InboundBizLogDto>(PublicConst.WmsLogApiUrl, "/BusinessLog/GetInboundBizLogByDays", query);
        }

        public ApiResponse<OutboundBizLogDto> GetOutboundBizLogByDays(CoreQuery query, int days)
        {
            query.ParmsObj = new { days };
            return ApiClient.Get<OutboundBizLogDto>(PublicConst.WmsLogApiUrl, "/BusinessLog/GetOutboundBizLogByDays", query);
        }

        public ApiResponse<LogStatisticBaseDto> GetHomePageAccessLogStatistic(CoreQuery query, int latestDays)
        {
            query.ParmsObj = new { latestDays };
            return ApiClient.Get<LogStatisticBaseDto>(PublicConst.WmsLogApiUrl, "/AccessLog/GetHomePageAccessLogStatistic", query);
        }

        public ApiResponse<LogStatisticBaseDto> GetHomePageInterfaceLogStatistic(CoreQuery query, int latestDays)
        {
            query.ParmsObj = new { latestDays };
            return ApiClient.Get<LogStatisticBaseDto>(PublicConst.WmsLogApiUrl, "/InterfaceLog/GetHomePageInterfaceLogStatistic", query);
        }

        public ApiResponse<ApiProcessResultTotalDto> GetApiProcessResult(CoreQuery query)
        {
            return ApiClient.Get<ApiProcessResultTotalDto>(PublicConst.WmsLogApiUrl, "/BusinessLog/GetApiProcessResult", query);
        }

        public ApiResponse<List<SummaryLogDto>> GetHomePageSummaryLog(CoreQuery query, SummaryLogQuery summaryLogQuery)
        {
            return ApiClient.Post<List<SummaryLogDto>>(PublicConst.WmsLogApiUrl, "/SummaryLog/GetHomePageSummaryLog", query, summaryLogQuery);
        }

        public ApiResponse<AccessLogDto> GetAccessLogViewDto(CoreQuery query, Guid sysId)
        {
            query.ParmsObj = new { sysId };
            return ApiClient.Get<AccessLogDto>(PublicConst.WmsLogApiUrl, "/AccessLog/GetAccessLogViewDto", query);
        }

        public ApiResponse<BusinessLogDto> GetBusinessLogViewDto(CoreQuery query, Guid sysId)
        {
            query.ParmsObj = new { sysId };
            return ApiClient.Get<BusinessLogDto>(PublicConst.WmsLogApiUrl, "/BusinessLog/GetBusinessLogViewDto", query);
        }

        public ApiResponse<InterfaceLogDto> GetInterfaceLogViewDto(CoreQuery query, Guid sysId)
        {
            query.ParmsObj = new { sysId };
            return ApiClient.Get<InterfaceLogDto>(PublicConst.WmsLogApiUrl, "/InterfaceLog/GetInterfaceLogViewDto", query);
        }

        public ApiResponse<List<SummaryLogDto>> GetHomePageMaxElapsedTimeLog(CoreQuery query)
        {
            return ApiClient.Get<List<SummaryLogDto>>(PublicConst.WmsLogApiUrl, "/SummaryLog/GetHomePageMaxElapsedTimeLog", query);
        }

        public ApiResponse<Pages<AccessLogListDto>> GetAccessLogList(CoreQuery query, AccessLogQuery accessLogQuery)
        {
            return ApiClient.Post<Pages<AccessLogListDto>>(PublicConst.WmsLogApiUrl, "/AccessLog/GetAccessLogList", query, accessLogQuery);
        }

        public ApiResponse<Pages<BusinessLogListDto>> GetBusinessLogList(CoreQuery query, BusinessLogQuery businessLogQuery)
        {
            return ApiClient.Post<Pages<BusinessLogListDto>>(PublicConst.WmsLogApiUrl, "/BusinessLog/GetBusinessLogList", query, businessLogQuery);
        }

        public ApiResponse<Pages<InterfaceLogListDto>> GetInterfaceLogList(CoreQuery query, InterfaceLogQuery interfaceLogQuery)
        {
            return ApiClient.Post<Pages<InterfaceLogListDto>>(PublicConst.WmsLogApiUrl, "/InterfaceLog/GetInterfaceLogList", query, interfaceLogQuery);
        }

        public ApiResponse<MaxFrequencyDto> GetHomePageMaxFrequencyLog(CoreQuery query)
        {
            return ApiClient.Get<MaxFrequencyDto>(PublicConst.WmsLogApiUrl, "/SummaryLog/GetHomePageMaxFrequencyLog", query);
        }

        public ApiResponse<MaxFrequencyLogDto> GetFrequencyDetailDto(CoreQuery query, string descr)
        {
            query.ParmsObj = new { descr };
            return ApiClient.Get<MaxFrequencyLogDto>(PublicConst.WmsLogApiUrl, "/SummaryLog/GetFrequencyDetailDto", query);
        }

        public ApiResponse<List<WarehouseDto>> GetWarehouseList(CoreQuery query, int userId)
        {
            query.ParmsObj = new { userId };
            return ApiClient.Post<List<WarehouseDto>>(PublicConst.WmsApiUrl, "/WareHouse/GetWareHouseByUserId", query);
        }

        /// <summary>
        /// 接口调用失败重新调用
        /// </summary>
        /// <returns></returns>
        public ApiResponse<bool> InvokeInterfaceLogApi(CoreQuery query, InterfaceLogQuery request)
        {
            return ApiClient.Post<bool>(PublicConst.WmsLogApiUrl, "/InterfaceLog/InvokeInterfaceLogApi", query, request);
        }

        /// <summary>
        /// 日志类型数据获取
        /// </summary>
        /// <param name="query"></param>
        /// <param name="interfaceLogQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<InterfaceStatisticList>> GetInterfaceStatisticByPage(CoreQuery query, InterfaceStatisticQuery dtoQuery)
        {
            return ApiClient.Post<Pages<InterfaceStatisticList>>(PublicConst.WmsLogApiUrl, "/SummaryLog/GetInterfaceStatisticByPage", query, dtoQuery);
        }

        /// <summary>
        /// 重新插入出库单
        /// </summary>
        /// <returns></returns>
        public ApiResponse<bool> InsertOutbound(CoreQuery query, InterfaceLogQuery request)
        {
            return ApiClient.Post<bool>(PublicConst.WmsLogApiUrl, "/InterfaceLog/InsertOutbound", query, request);
        }
    }
}