using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBK.WMS.Portal.Services
{
    public class OrderManegerApiClient
    {
        private static readonly OrderManegerApiClient instance = new OrderManegerApiClient();

        private OrderManegerApiClient()
        {
        }

        public static OrderManegerApiClient GetInstance()
        {
            return instance;
        }

        #region 移仓单管理

        #region 移仓单查询
        /// <summary>
        /// 移仓单列表查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<TransferinventoryListDto>> GetTransferinventoryByPage(TransferinventoryQuery request)
        {
            var query = new CoreQuery();
            return ApiClient.Post<Pages<TransferinventoryListDto>>(PublicConst.WmsApiUrl, "/Order/TransferInventoryWebApi/GetTransferinventoryByPage", query, request);
        }
        #endregion 移仓单管理

        #region 获取移仓单
        /// <summary>
        /// 获取移仓单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<TransferInventoryViewDto> GetTransferinventoryBySysId(Guid sysId, Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Post<TransferInventoryViewDto>(PublicConst.WmsApiUrl, "/Order/TransferInventoryWebApi/GetTransferinventoryBySysId", query);
        }
        #endregion 获取移仓单

        /// <summary>
        /// 作废移仓单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<bool> ObsoleteTransferinventory(TransferinventoryUpdateQuery dto)
        {
            var query = new CoreQuery();
            return ApiClient.Post<bool>(PublicConst.WmsApiUrl, "/Order/TransferInventoryWebApi/ObsoleteTransferinventory", query, dto);
        }

        #endregion

        #region 预包装

        #region 预包装查询
        /// <summary>
        /// 预包装单子查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<PrePackListDto>> GetPrePackByPage(PrePackQuery request)
        {
            var query = new CoreQuery();
            return ApiClient.Post<Pages<PrePackListDto>>(PublicConst.WmsApiUrl, "/Order/PrePack/GetPrePackByPage", query, request);
        }
        #endregion

        #region 预包装库存
        /// <summary>
        /// 预包装商品库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<PrePackSkuListDto>> GetPrePackSkuByPage(PrePackSkuQuery request)
        {
            var query = new CoreQuery();
            return ApiClient.Post<Pages<PrePackSkuListDto>>(PublicConst.WmsApiUrl, "/Order/PrePack/GetPrePackSkuByPage", query, request);
        }
        #endregion

        #region 新增预包装
        /// <summary>
        /// 新增预包装单
        /// </summary>
        /// <param name="prePackSkuDto"></param>
        /// <returns></returns>
        public ApiResponse SavePrePackSku(PrePackSkuDto prePackSkuDto)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Order/PrePack/SavePrePackSku", new CoreQuery(), prePackSkuDto);
        }
        #endregion

        #region 获取预包装单详情
        public ApiResponse<PrePackSkuDto> GetPrePackBySysId(Guid sysId, Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new
            {
                sysId,
                warehouseSysId
            };
            return ApiClient.Post<PrePackSkuDto>(PublicConst.WmsApiUrl, "/Order/PrePack/GetPrePackBySysId", query);
        }
        #endregion

        #region 编辑预包装
        /// <summary>
        /// 编辑预包装
        /// </summary>
        /// <param name="prePackSkuDto"></param>
        /// <returns></returns>
        public ApiResponse UpdatePrePackSku(PrePackSkuDto prePackSkuDto)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Order/PrePack/UpdatePrePackSku", new CoreQuery(), prePackSkuDto);
        }
        #endregion

        #region 删除预包装
        /// <summary>
        /// 删除预包装
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<string> DeletePerPack(List<Guid> sysId, Guid warehouseSysId)
        {
            var coreQuery = new CoreQuery();
            coreQuery.ParmsObj = new { warehouseSysId };
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "/Order/PrePack/DeletePerPack", coreQuery, sysId);
        }
        #endregion

        #region 预包装导入
        /// <summary>
        /// 预包装导入
        /// </summary>
        /// <param name="prePackSkuDto"></param>
        /// <returns></returns>
        public ApiResponse ImportPrePack(PrePackSkuDto prePackSkuDto)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Order/PrePack/ImportPrePack", new CoreQuery(), prePackSkuDto);
        }
        #endregion

        #endregion

        #region 散货封装

        public ApiResponse<Pages<PreBulkPackDto>> GetPreBulkPackByPage(PreBulkPackQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<PreBulkPackDto>>(PublicConst.WmsApiUrl, "/Order/PreBulkPack/GetPreBulkPackByPage", query, request);
        }

        /// <summary>
        /// 增加散货预包装
        /// </summary>
        /// <param name="batchPreBulkPackDto"></param>
        /// <returns></returns>
        public ApiResponse<bool> AddPreBulkPack(BatchPreBulkPackDto batchPreBulkPackDto)
        {
            return ApiClient.Post<bool>(PublicConst.WmsApiUrl, "/Order/PreBulkPack/AddPreBulkPack", new CoreQuery(), batchPreBulkPackDto);
        }

        public ApiResponse<PreBulkPackDto> GetPreBulkPackBySysId(Guid sysId, Guid warehouseSysId, CoreQuery query)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<PreBulkPackDto>(PublicConst.WmsApiUrl, "/Order/PreBulkPack/GetPreBulkPackBySysId", query);
        }

        public ApiResponse UpdatePreBulkPack(PreBulkPackDto request, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Order/PreBulkPack/UpdatePreBulkPack", query, request);
        }

        /// <summary>
        /// 删除损益单商品
        /// </summary>
        /// <param name="sysIds"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse DeletePrebulkPackSkus(List<Guid> sysIds, CoreQuery query, Guid warehouseSysId)
        {
            query.ParmsObj = new { warehouseSysId };
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Order/PreBulkPack/DeletePrebulkPackSkus", query, sysIds);
        }

        /// <summary>
        /// 删除损益单
        /// </summary>
        /// <param name="sysIds"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse DeletePrebulkPack(List<Guid> sysIds, CoreQuery query, Guid warehouseSysId)
        {
            query.ParmsObj = new { warehouseSysId };

            return ApiClient.Post(PublicConst.WmsApiUrl, "/Order/PreBulkPack/DeletePrebulkPack", query, sysIds);
        }

        /// <summary>
        /// 散货导入
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ApiResponse ImportPreBulkPack(PreBulkPackDto dto)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Order/PreBulkPack/ImportPreBulkPack", new CoreQuery(), dto);
        }

        /// <summary>
        /// 根据出库单ID获取散货封箱单相贴
        /// </summary>
        /// <param name="sysId">出库单Id</param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<List<string>> GetPrebulkPackStorageCase(Guid sysId, CoreQuery query, Guid warehouseSysId)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<List<string>>(PublicConst.WmsApiUrl, "/Order/PreBulkPack/GetPrebulkPackStorageCase", query);
        }
        #endregion

        #region 判断货位是否存在
        public ApiResponse<string> IsStorageLoc(PrePackQuery query)
        {
            var result = ApiClient.Post<string>(PublicConst.WmsApiUrl, "/Order/PrePack/IsStorageLoc", new CoreQuery(), query);
            return result;
        }
        #endregion

        #region 复制预包装
        /// <summary>
        /// 复制预包装
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<string> CopyPrePack(PrePackCopy query)
        {
            var result = ApiClient.Post<string>(PublicConst.WmsApiUrl, "/Order/PrePack/CopyPrePack", new CoreQuery(), query);
            return result;
        }
        #endregion

        #region 交接管理

        public ApiResponse<Pages<OutboundTransferOrderDto>> GetOutboundTransferOrderByPage(OutboundTransferOrderQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<OutboundTransferOrderDto>>(PublicConst.WmsApiUrl, "/Order/OutboundTransferOrder/GetOutboundTransferOrderByPage", query, request);
        }

        public ApiResponse<OutboundTransferOrderDto> GetDataBySysId(Guid sysId, Guid warehouseSysId, CoreQuery query)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<OutboundTransferOrderDto>(PublicConst.WmsApiUrl, "/Order/OutboundTransferOrder/GetDataBySysId", query);
        }

        public ApiResponse<List<OutboundTransferPrintDto>> GetOutboundTransferBox(List<Guid> sysIds, Guid warehouseSysId, CoreQuery query)
        {
            query.ParmsObj = new { warehouseSysId };
            return ApiClient.Post<List<OutboundTransferPrintDto>>(PublicConst.WmsApiUrl, "Order/OutboundTransferOrder/GetOutboundTransferBox", query, sysIds);
        }

        /// <summary>
        /// 根据出库单获取所有交接单
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="warehouseSysId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<List<OutboundTransferPrintDto>> GetOutboundTransferOrder(OutboundTransferOrderQuery dto, CoreQuery query)
        {
            return ApiClient.Post<List<OutboundTransferPrintDto>>(PublicConst.WmsApiUrl, "Order/OutboundTransferOrder/GetOutboundTransferOrder", query, dto);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<CommonResponse> UpdateTransferOrderSku(OutboundTransferOrderMoveDto request, CoreQuery query)
        {
            return ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "/Order/OutboundTransferOrder/UpdateTransferOrderSku", query, request);
        }
        #endregion
    }
}