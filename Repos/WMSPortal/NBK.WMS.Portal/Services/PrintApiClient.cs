using System.Collections.Generic;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using System;

namespace NBK.WMS.Portal.Services
{
    public class PrintApiClient
    {
        private static readonly PrintApiClient instance = new PrintApiClient();

        private PrintApiClient()
        {
        }

        public static PrintApiClient GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// 按单打印
        /// </summary>
        /// <param name="pickDetailOrder"></param>
        /// <returns></returns>
        public ApiResponse<PrintPickDetailByOrderDto> GetPrintPickDetailByOrderDto(string pickDetailOrder, Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { pickDetailOrder, warehouseSysId };
            return ApiClient.Get<PrintPickDetailByOrderDto>(PublicConst.WmsApiUrl, "/Print/GetPrintPickDetailByOrderDto", query);
        }

        /// <summary>
        /// 批量打印
        /// </summary>
        /// <param name="pickDetailOrder"></param>
        /// <returns></returns>
        public ApiResponse<PrintPickDetailByBatchDto> GetPrintPickDetailByBatchDto(string pickDetailOrder, Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { pickDetailOrder, warehouseSysId };
            return ApiClient.Get<PrintPickDetailByBatchDto>(PublicConst.WmsApiUrl, "/Print/GetPrintPickDetailByBatchDto", query);
        }

        public ApiResponse<PrintPickDetailByOrderDto> GetPrintRecommendPickDetail(string outboundSysId, Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { outboundSysId, warehouseSysId };
            return ApiClient.Get<PrintPickDetailByOrderDto>(PublicConst.WmsApiUrl, "/Print/GetPrintRecommendPickDetail", query);
        }

        /// <summary>
        /// 收货打印
        /// </summary>
        /// <param name="receiptOrder"></param>
        /// <returns></returns>
        public ApiResponse<PrintReceiptDto> GetPrintReceiptDto(string receiptOrder, Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { receiptOrder, warehouseSysId };
            return ApiClient.Get<PrintReceiptDto>(PublicConst.WmsApiUrl, "/Print/GetPrintReceiptDto", query);
        }

        /// <summary>
        /// 打印采购单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<PrintPurchaseViewDto> GetPrintPurchaseViewDto(CoreQuery query, string sysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<PrintPurchaseViewDto>(PublicConst.WmsApiUrl, "/Print/GetPrintPurchaseViewDto", query);
        }

        /// <summary>
        /// 打印装箱单
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<PrintVanningDetailDto> GetPrintVanningDetailDto(string vanningDetailSysId, string actionType, CoreQuery query, Guid warehouseSysId)
        {
            query.ParmsObj = new { vanningDetailSysId, actionType, warehouseSysId };
            return ApiClient.Get<PrintVanningDetailDto>(PublicConst.WmsApiUrl, "/Print/GetPrintVanningDetailDto", query);
        }

        /// <summary>
        /// 获取箱贴数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<PrintVanningDetailStickDto> GetPrintVanningDetailStick(string vanningDetailSysId, string actionType, CoreQuery query, Guid warehouseSysId)
        {
            query.ParmsObj = new { vanningDetailSysId, actionType, warehouseSysId };
            return ApiClient.Get<PrintVanningDetailStickDto>(PublicConst.WmsApiUrl, "/Print/GetPrintVanningDetailStick", query);
        }

        /// <summary>
        /// 获取箱贴数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<PrintVanningDetailStickDto> GetPrintVanningDetailStickToB(string vanningDetailSysId, string actionType, CoreQuery query, Guid warehouseSysId)
        {
            query.ParmsObj = new { vanningDetailSysId, actionType, warehouseSysId };
            return ApiClient.Get<PrintVanningDetailStickDto>(PublicConst.WmsApiUrl, "/Print/GetPrintVanningDetailStickToB", query);
        }

        /// <summary>
        /// 获取打印出库单数据
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <param name="wareHouseSysId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<PrintOutboundDto> GetPrintOutboundDto(Guid sysId, CoreQuery query, Guid warehouseSysId)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<PrintOutboundDto>(PublicConst.WmsApiUrl, "/Print/GetPrintOutboundDto", query);
        }

        /// <summary>
        /// 获取打印预包装差异数据
        /// </summary>
        /// <param name="query"></param>
        /// <param name="outboundOrder"></param>
        /// <returns></returns>
        public ApiResponse<PrintOutboundPrePackDiffDto> GetPrintOutboundPrePackDiffDto(CoreQuery query, Guid sysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<PrintOutboundPrePackDiffDto>(PublicConst.WmsApiUrl, "/Print/GetPrintOutboundPrePackDiffDto", query);
        }


        /// <summary>
        /// 获取打印出库单对应的交接单列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="outboundOrder"></param>
        /// <returns></returns>
        public ApiResponse<List<PrintTMSPackNumberListDto>> GetPrintTMSPackNumberList(Guid sysId, CoreQuery query, Guid warehouseSysId)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<List<PrintTMSPackNumberListDto>>(PublicConst.WmsApiUrl, "/Print/GetPrintTMSPackNumberList", query);
        }

        /// <summary>
        /// 获取打印散货箱差异数据
        /// </summary>
        /// <param name="query"></param>
        /// <param name="outboundOrder"></param>
        /// <returns></returns>
        public ApiResponse<PrintOutboundPrePackDiffDto> GetPrintOutboundPreBulkPackDiffDto(CoreQuery query, Guid sysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<PrintOutboundPrePackDiffDto>(PublicConst.WmsApiUrl, "/Print/GetPrintOutboundPreBulkPackDiffDto", query);
        }

        /// <summary>
        /// 生产加工单推荐拣货货位
        /// </summary>
        /// <param name="query"></param>
        /// <param name="assemblySysId"></param>
        /// <returns></returns>
        public ApiResponse<PrintAssemblyRCMDPickDetailDto> GetPrintAssemblyRCMDPickDetail(CoreQuery query, string assemblySysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { assemblySysId, warehouseSysId };
            return ApiClient.Get<PrintAssemblyRCMDPickDetailDto>(PublicConst.WmsApiUrl, "/Print/GetPrintAssemblyRCMDPickDetail", query);
        }


        /// <summary>
        /// 生产加工单推荐拣货货位
        /// </summary>
        /// <param name="query"></param>
        /// <param name="assemblySysId"></param>
        /// <returns></returns>
        public ApiResponse<PrintAutoShelvesDto> PrintReceiptAutoShelves(CoreQuery query, Guid receiptSysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { receiptSysId, warehouseSysId };
            return ApiClient.Get<PrintAutoShelvesDto>(PublicConst.WmsApiUrl, "/Print/GetPrintReceiptAutoShelves", query);
        }

        /// <summary>
        /// 打印盘点单
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<PrintStockTakeDto> GetPrintStockTakeDto(CoreQuery query, Guid sysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<PrintStockTakeDto>(PublicConst.WmsApiUrl, "/Print/GetPrintStockTakeDto", query);
        }

        /// <summary>
        /// 打印盘点单汇总报告
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<PrintStockTakeReportDto> GetPrintStockTakeReportDto(CoreQuery query, List<Guid> sysIds, Guid warehouseSysId)
        {
            query.ParmsObj = new {  warehouseSysId };
            return ApiClient.Post<PrintStockTakeReportDto>(PublicConst.WmsApiUrl, "/Print/GetPrintStockTakeReportDto", query, sysIds);
        }

        /// <summary>
        /// 打印质检单
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<PrintQualityControlDto> GetPrintQualityControlDto(CoreQuery query, Guid sysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<PrintQualityControlDto>(PublicConst.WmsApiUrl, "/Print/GetPrintQualityControlDto", query);
        }

        /// <summary>
        /// 批量打印散货封箱单
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        public ApiResponse<PrintPrebulkPackByBatchDto> GetPrintPrebulkPackByBatchDto(CoreQuery query, List<Guid> sysIds, Guid warehouseSysId)
        {
            query.ParmsObj = new { warehouseSysId };
            return ApiClient.Post<PrintPrebulkPackByBatchDto>(PublicConst.WmsApiUrl, "/Print/GetPrintPrebulkPackByBatchDto", query, sysIds);
        }

        /// <summary>
        /// 获取打印领料数据
        /// </summary>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<PrintPickingMaterialDto> GetPrintPickingMaterialDetailList(CoreQuery query, PrintPickingMaterialQuery request)
        {
            return ApiClient.Post<PrintPickingMaterialDto>(PublicConst.WmsApiUrl, "/Print/GetPrintPickingMaterialDetailList", query, request);
        }
    }
}