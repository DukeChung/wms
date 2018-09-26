using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.ThirdParty;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;

namespace NBK.WMS.Portal.Services
{
    public class InventoryApiClient
    {
        private static readonly InventoryApiClient instance = new InventoryApiClient();

        private InventoryApiClient()
        {
        }

        public static InventoryApiClient GetInstance()
        {

            return instance;
        }

        #region 损益

        /// <summary>
        /// 分页获取损益
        /// </summary>
        /// <param name="adjustmentQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<AdjustmentListDto>> GetAdjustmentListByPage(AdjustmentQuery adjustmentQuery, CoreQuery query)
        {
            return ApiClient.Post<Pages<AdjustmentListDto>>(PublicConst.WmsApiUrl, "/Inventory/Adjustment/GetAdjustmentListByPage", query, adjustmentQuery);
        }

        /// <summary>
        /// 获取损益明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<AdjustmentViewDto> GetAdjustmentBySysId(Guid sysId, Guid warehouseSysId, CoreQuery query)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<AdjustmentViewDto>(PublicConst.WmsApiUrl, "/Inventory/Adjustment/GetAdjustmentBySysId", query);
        }

        /// <summary>
        /// 创建损益
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public ApiResponse AddAdjustment(AdjustmentDto adjustment, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inventory/Adjustment/AddAdjustment", query, adjustment);
        }

        /// <summary>
        /// 编辑损益单
        /// </summary>
        /// <param name="adjustment"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse UpdateAdjustment(AdjustmentDto adjustment, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inventory/Adjustment/UpdateAdjustment", query, adjustment);
        }

        /// <summary>
        /// 删除损益单商品
        /// </summary>
        /// <param name="sysIds"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse DeleteAjustmentSkus(List<Guid> sysIds, Guid warehouseSysId, CoreQuery query)
        {
            query.ParmsObj = new { warehouseSysId };
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inventory/Adjustment/DeleteAjustmentSkus", query, sysIds);
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="adjustment"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse Audit(AdjustmentAuditDto adjustment, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inventory/Adjustment/Audit", query, adjustment);
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="adjustment"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse Void(AdjustmentAuditDto adjustment, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inventory/Adjustment/Void", query, adjustment);
        }

        /// <summary>
        /// 获取商品库存分页
        /// </summary>
        /// <param name="adjustmentQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<SkuInvLotLocLpnDto>> GetAdjustmentListByPage(SkuInvLotLocLpnQuery adjustmentQuery, CoreQuery query)
        {
            return ApiClient.Post<Pages<SkuInvLotLocLpnDto>>(PublicConst.WmsApiUrl, "/Inventory/Adjustment/GetSkuInventoryList", query, adjustmentQuery);
        }


        #endregion

        #region 盘点
        /// <summary>
        /// 获取盘点列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<StockTakeListDto>> GetStockTakeList(CoreQuery query, StockTakeQuery stockTakeQuery)
        {
            return ApiClient.Post<Pages<StockTakeListDto>>(PublicConst.WmsApiUrl, "/Inventory/StockTake/GetStockTakeList", query, stockTakeQuery);
        }

        /// <summary>
        /// 获取待盘点商品信息
        /// </summary>
        /// <param name="query"></param>
        /// <param name="stockTakeSkuQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<StockTakeSkuListDto>> GetWaitingStockTakeSkuList(CoreQuery query, StockTakeSkuQuery stockTakeSkuQuery)
        {
            return ApiClient.Post<Pages<StockTakeSkuListDto>>(PublicConst.WmsApiUrl, "/Inventory/StockTake/GetWaitingStockTakeSkuList", query, stockTakeSkuQuery);
        }

        /// <summary>
        /// 创建盘点单
        /// </summary>
        /// <param name="query"></param>
        /// <param name="stockTakeDto"></param>
        /// <returns></returns>
        public ApiResponse NewStockTake(CoreQuery query, NewStockTakeDto newStockTakeDto)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inventory/StockTake/NewStockTake", query, newStockTakeDto);
        }

        /// <summary>
        /// 开始盘点
        /// </summary>
        /// <param name="query"></param>
        /// <param name="stockTakeStartDto"></param>
        /// <returns></returns>
        public ApiResponse StockTakeStart(CoreQuery query, StockTakeStartDto stockTakeStartDto)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inventory/StockTake/StockTakeStart", query, stockTakeStartDto);
        }

        /// <summary>
        /// 创建盘点单
        /// </summary>
        /// <param name="query"></param>
        /// <param name="stockTakeDto"></param>
        /// <returns></returns>
        [Obsolete]
        public ApiResponse<Guid> AddStockTake(CoreQuery query, StockTakeDto stockTakeDto)
        {
            return ApiClient.Post<Guid>(PublicConst.WmsApiUrl, "/Inventory/StockTake/AddStockTake", query, stockTakeDto);
        }

        /// <summary>
        /// 获取仓库下拉框
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<List<SelectItem>> SelectItemWarehouse(CoreQuery query)
        {
            return ApiClient.Get<List<SelectItem>>(PublicConst.WmsApiUrl, "/Inventory/StockTake/GetSelectWarehouse", query);
        }

        /// <summary>
        /// 获取盘点单
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<StockTakeViewDto> GetStockTakeViewById(CoreQuery query, string sysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<StockTakeViewDto>(PublicConst.WmsApiUrl, "/Inventory/StockTake/GetStockTakeViewById", query);
        }

        /// <summary>
        /// 获取盘点单明细
        /// </summary>
        /// <param name="query"></param>
        /// <param name="stockTakeViewQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<StockTakeDetailViewDto>> GetStockTakeDetailList(CoreQuery query, StockTakeViewQuery stockTakeViewQuery)
        {
            return ApiClient.Post<Pages<StockTakeDetailViewDto>>(PublicConst.WmsApiUrl, "/Inventory/StockTake/GetStockTakeDetailList", query, stockTakeViewQuery);
        }

        /// <summary>
        /// 获取盘点单差异
        /// </summary>
        /// <param name="query"></param>
        /// <param name="stockTakeViewQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<StockTakeDetailViewDto>> GetStockTakeDiffList(CoreQuery query, StockTakeViewQuery stockTakeViewQuery)
        {
            return ApiClient.Post<Pages<StockTakeDetailViewDto>>(PublicConst.WmsApiUrl, "/Inventory/StockTake/GetStockTakeDiffList", query, stockTakeViewQuery);
        }

        /// <summary>
        /// 盘点单明细复盘
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        public ApiResponse<dynamic> ReplayStockTakeDetail(CoreQuery query, ReplayStockTakeDto replayStockTakeDto)
        {
            return ApiClient.Post<dynamic>(PublicConst.WmsApiUrl, "/Inventory/StockTake/ReplayStockTakeDetail", query, replayStockTakeDto);
        }

        /// <summary>
        /// 获取生成损益单数据
        /// </summary>
        /// <param name="query"></param>
        /// <param name="createAdjustmentDto"></param>
        /// <returns></returns>
        public ApiResponse<AdjustmentDto> GetAdjustmentDto(CoreQuery query, CreateAdjustmentDto createAdjustmentDto)
        {
            return ApiClient.Post<AdjustmentDto>(PublicConst.WmsApiUrl, "/Inventory/StockTake/GetAdjustmentDto", query, createAdjustmentDto);
        }

        /// <summary>
        /// 获取商品分类下拉框
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parentSysId"></param>
        /// <returns></returns>
        public ApiResponse<List<SelectItem>> SelectItemSkuClass(CoreQuery query, Guid? parentSysId)
        {
            return ApiClient.Post<List<SelectItem>>(PublicConst.WmsApiUrl, "/Inventory/StockTake/GetSelectSkuClass", query, new SelectSkuClassDto { ParentSysId = parentSysId });
        }

        /// <summary>
        /// 盘点完成
        /// </summary>
        /// <param name="query"></param>
        /// <param name="stockTakeCompleteDto"></param>
        /// <returns></returns>
        public ApiResponse<bool> StockTakeComplete(CoreQuery query, StockTakeCompleteDto stockTakeCompleteDto)
        {
            return ApiClient.Post<bool>(PublicConst.WmsApiUrl, "/Inventory/StockTake/StockTakeComplete", query, stockTakeCompleteDto);
        }

        /// <summary>
        /// 删除盘点单
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        public ApiResponse DeleteStockTake(CoreQuery query, List<Guid> sysIds)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inventory/StockTake/DeleteStockTake", query, sysIds);
        }

        public ApiResponse<Pages<StockTakeReportListDto>> GetStockTakeReport(CoreQuery query, StockTakeReportQuery stockTakeReportQuery)
        {
            return ApiClient.Post<Pages<StockTakeReportListDto>>(PublicConst.WmsApiUrl, "/Inventory/StockTake/GetStockTakeReport", query, stockTakeReportQuery);
        }
        #endregion

        #region 库存移动
        /// <summary>
        /// 获取库存移动SKU列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="stockMovementSkuQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<StockMovementSkuDto>> GetStockMovementSkuList(CoreQuery query, StockMovementSkuQuery stockMovementSkuQuery)
        {
            return ApiClient.Post<Pages<StockMovementSkuDto>>(PublicConst.WmsApiUrl, "/Inventory/StockMovement/GetStockMovementSkuList", query, stockMovementSkuQuery);
        }

        /// <summary>
        /// 获取库存移动信息
        /// </summary>
        /// <param name="query"></param>
        /// <param name="skuSysId"></param>
        /// <param name="loc"></param>
        /// <param name="lot"></param>
        /// <returns></returns>
        public ApiResponse<StockMovementDto> GetStockMovement(CoreQuery query, string skuSysId, string loc, string lot, Guid wareHouseSysId)
        {
            query.ParmsObj = new { skuSysId, loc, lot, wareHouseSysId };
            return ApiClient.Get<StockMovementDto>(PublicConst.WmsApiUrl, "/Inventory/StockMovement/GetStockMovement", query);
        }

        /// <summary>
        /// 保存调整
        /// </summary>
        /// <param name="query"></param>
        /// <param name="stockMovementDto"></param>
        /// <returns></returns>
        public ApiResponse<dynamic> SaveStockMovement(CoreQuery query, StockMovementDto stockMovementDto)
        {
            return ApiClient.Post<dynamic>(PublicConst.WmsApiUrl, "/Inventory/StockMovement/SaveStockMovement", query, stockMovementDto);
        }

        /// <summary>
        /// 获取移动单列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="stockMovementQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<StockMovementDto>> GetStockMovementList(CoreQuery query, StockMovementQuery stockMovementQuery)
        {
            return ApiClient.Post<Pages<StockMovementDto>>(PublicConst.WmsApiUrl, "/Inventory/StockMovement/GetStockMovementList", query, stockMovementQuery);
        }

        /// <summary>
        /// 确认移动
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        public ApiResponse ConfirmStockMovement(CoreQuery query, StockMovementOperationDto stockMovementOperationDto)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inventory/StockMovement/ConfirmStockMovement", query, stockMovementOperationDto);
        }

        /// <summary>
        /// 取消移动
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        public ApiResponse CancelStockMovement(CoreQuery query, StockMovementOperationDto stockMovementOperationDto)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inventory/StockMovement/CancelStockMovement", query, stockMovementOperationDto);
        }

        /// <summary>
        /// 库位变更导入
        /// </summary>
        /// <param name="query"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public ApiResponse ImportStockMovementList(CoreQuery query, ImportStockMovement list)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inventory/StockMovement/ImportStockMovementList", query, list);
        }

        #endregion

        #region 库存转移

        /// <summary>
        /// 获取当前库存批次信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<StockTransferLotListDto>> GetStockTransferLotByPage(StockTransferQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<StockTransferLotListDto>>(PublicConst.WMSReportUrl, "/OtherRead/GetStockTransferLotByPage", query, request);
        }

        /// <summary>
        /// 获取待转移批次商品信息
        /// </summary>
        /// <param name="invlotloclpnSysId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<StockTransferDto> GetStockTransferBySysId(Guid invlotloclpnSysId, Guid warehouseSysId, CoreQuery query)
        {
            query.ParmsObj = new { sysId = invlotloclpnSysId, warehouseSysId };
            return ApiClient.Get<StockTransferDto>(PublicConst.WmsApiUrl, "/Inventory/StockTransfer/GetStockTransferBySysId", query);
        }

        /// <summary>
        /// 创建批次转移单
        /// </summary>
        /// <param name="uom"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse CreateStockTransfer(StockTransferDto st, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inventory/StockTransfer/CreateStockTransfer", query, st);
        }

        /// <summary>
        /// 分页获取批次转移单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<StockTransferDto>> GetStockTransferOrderByPage(StockTransferQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<StockTransferDto>>(PublicConst.WmsApiUrl, "/Inventory/StockTransfer/GetStockTransferOrderByPage", query, request);
        }

        /// <summary>
        /// 批次确认转移
        /// </summary>
        /// <param name="st"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse StockTransferOperation(StockTransferDto request, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inventory/StockTransfer/StockTransferOperation", query, request);
        }

        /// <summary>
        /// 取消转移
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse StockTransferCancel(StockTransferDto request, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inventory/StockTransfer/StockTransferCancel", query, request);
        }

        /// <summary>
        /// 获取转移单明细内容
        /// </summary>
        /// <param name="invlotloclpnSysId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<StockTransferDto> GetStockTransferOrderBySysId(Guid sysId, Guid warehouseSysId, CoreQuery query)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<StockTransferDto>(PublicConst.WmsApiUrl, "/Inventory/StockTransfer/GetStockTransferOrderBySysId", query);
        }
        #endregion

        #region 库存冻结

        public ApiResponse<Pages<FrozenRequestSkuDto>> GetFrozenRequestSkuByPage(FrozenRequestQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<FrozenRequestSkuDto>>(PublicConst.WmsApiUrl, "/Inventory/Frozen/GetFrozenRequestSkuByPage", query, request);
        }

        /// <summary>
        /// 库存冻结
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse SaveFrozenRequest(FrozenRequestDto request, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inventory/Frozen/SaveFrozenRequest", query, request);
        }

        public ApiResponse<Pages<FrozenListDto>> GetFrozenRequestList(FrozenListQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<FrozenListDto>>(PublicConst.WmsApiUrl, "/Inventory/Frozen/GetFrozenRequestList", query, request);
        }

        public ApiResponse UnFrozenRequest(FrozenRequestDto request, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inventory/Frozen/UnFrozenRequest", query, request);
        }

        public ApiResponse<Pages<FrozenRequestSkuDto>> GetFrozenDetailByPage(FrozenRequestQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<FrozenRequestSkuDto>>(PublicConst.WmsApiUrl, "/Inventory/Frozen/GetFrozenDetailByPage", query, request);
        }
        #endregion

        public ApiResponse<List<ThirdPartyStockTransferDto>> GetInitChannelInventoryData(InitInventoryFromChannelRequest request, CoreQuery query)
        {
            return ApiClient.Post<List<ThirdPartyStockTransferDto>>(PublicConst.WmsApiUrl, "/ThirdParty/GetInitChannelInventoryData", query, request);
        }

        public ApiResponse<CommonResponse> InsertStockTransfer(ThirdPartyStockTransferDto request, CoreQuery query)
        {
            return ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "/ThirdParty/InsertStockTransfer", query, request);
        }
    }
}