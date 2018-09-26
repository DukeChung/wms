using System.Collections.Generic;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using System;

namespace NBK.WMS.Portal.Services
{
    public class ChartApiClient
    {
        private static readonly ChartApiClient instance = new ChartApiClient();

        private ChartApiClient()
        {
        }

        public static ChartApiClient GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// 根据入库单SysId获取数据
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public ApiResponse<PurchaseAndOutboundChartDto> GetPurchaseAndOutboundChart(Guid wareHouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { wareHouseSysId };
            return ApiClient.Post<PurchaseAndOutboundChartDto>(PublicConst.WMSReportUrl, "/Chart/GetPurchaseAndOutboundChart", query);
        }

        /// <summary>
        /// 获取临期Sku
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ApiResponse<List<AdventSkuChartDto>> GetAdventSkuChartTop5(Guid wareHouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { wareHouseSysId };
            return ApiClient.Post<List<AdventSkuChartDto>>(PublicConst.WMSReportUrl, "/Chart/GetAdventSkuChartTop5", query);
        }

        /// <summary>
        /// 过去十天收发货商品数量
        /// </summary>
        /// <returns></returns>
        public ApiResponse<List<SkuReceiptOutboundReportDto>> GetSkuReceiptOutboundChartAfter10(CoreQuery query, Guid wareHouseSysId)
        {
            query.ParmsObj = new { wareHouseSysId };
            return ApiClient.Get<List<SkuReceiptOutboundReportDto>>(PublicConst.WMSReportUrl, "/Chart/GetSkuReceiptOutboundChartAfter10", query);
        }

        /// <summary>
        /// 获取过去十天订单与退货数量
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<List<OutboundAndReturnCharDto>> GetOutboundAndReturnCharDataOfLastTenDays(CoreQuery query, Guid wareHouseSysId)
        {
            query.ParmsObj = new { wareHouseSysId };
            return ApiClient.Get<List<OutboundAndReturnCharDto>>(PublicConst.WMSReportUrl, "/Chart/GetOutboundAndReturnCharDataOfLastTenDays", query);
        }

        /// <summary>
        /// 获取12条预包装单
        /// </summary>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        public ApiResponse<List<PrePackBoardDto>> GetPrePackBoardTop12(Guid wareHouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { wareHouseSysId };
            return ApiClient.Get<List<PrePackBoardDto>>(PublicConst.WMSReportUrl, "/Chart/GetPrePackBoardTop12", query);
        }

        public ApiResponse<OutboundTotalChartDto> GetToDayOrderStatusTotal(Guid wareHouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { wareHouseSysId };
            return ApiClient.Get<OutboundTotalChartDto>(PublicConst.WMSReportUrl, "/Chart/GetToDayOrderStatusTotal", query);
        }

        /// <summary>
        /// 获取12条预包装单
        /// </summary>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        public ApiResponse<List<ExceedThreeDaysPurchase>> GetExceedThreeDaysPurchase(Guid wareHouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { wareHouseSysId };
            return ApiClient.Get<List<ExceedThreeDaysPurchase>>(PublicConst.WMSReportUrl, "/Chart/GetExceedThreeDaysPurchase", query);
        }

    }
}