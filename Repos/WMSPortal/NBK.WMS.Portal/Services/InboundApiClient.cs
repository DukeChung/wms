using System;
using System.Collections.Generic;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.DTO.Receipt;

namespace NBK.WMS.Portal.Services
{
    public class InboundApiClient
    {
        private static readonly InboundApiClient instance = new InboundApiClient();

        private InboundApiClient()
        {
        }

        public static InboundApiClient GetInstance()
        {
            return instance;
        }

        #region 采购

        /// <summary>
        /// 采购
        /// </summary> 
        /// <param name="purchaseQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<PurchaseListDto>> GetPurchaseDtoList(PurchaseQuery purchaseQuery)
        {
            var query = new CoreQuery();

            return ApiClient.Post<Pages<PurchaseListDto>>(PublicConst.WMSReportUrl, "OtherRead/GetPurchaseDtoList", query, purchaseQuery);
        }

        /// <summary>
        /// 退货入库
        /// </summary>
        /// <param name="purchaseQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<PurchaseReturnListDto>> GetPurchaseDtoReturnList(PurchaseReturnQuery purchaseQuery)
        {
            var query = new CoreQuery();

            return ApiClient.Post<Pages<PurchaseReturnListDto>>(PublicConst.WMSReportUrl, "OtherRead/GetPurchaseReturnDtoList", query, purchaseQuery);
        }

        /// <summary>
        /// 获取采购单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<PurchaseViewDto> GetPurchaseViewDtoBySysId(Guid sysId,Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Post<PurchaseViewDto>(PublicConst.WMSReportUrl, "OtherRead/GetPurchaseViewDtoBySysId", query);
        }

        /// <summary>
        /// 获取退货入库单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<PurchaseReturnViewDto> GetPurchaseReturnViewDtoBySysId(Guid sysId, Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Post<PurchaseReturnViewDto>(PublicConst.WMSReportUrl, "OtherRead/GetPurchaseReturnViewDtoBySysId", query);
        }

        /// <summary>
        /// 指定入库批号
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse AppointBatchNumber(string sysId,string batchNumber, Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysId, batchNumber, warehouseSysId };
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inbound/Purchase/AppointBatchNumber", query);
        }

        /// <summary>
        /// 作废采购订单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<bool> ObsoletePurchase(PurchaseOperateDto request, CoreQuery query)
        {
            return ApiClient.Post<bool>(PublicConst.WmsApiUrl, "/Inbound/Purchase/ObsoletePurchase", query, request);
        }

        /// <summary>
        /// 关闭采购订单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<bool> ClosePurchase(PurchaseOperateDto request, CoreQuery query)
        {
            return ApiClient.Post<bool>(PublicConst.WmsApiUrl, "/Inbound/Purchase/ClosePurchase", query, request);
        }

        /// <summary>
        /// 退货入库，自动收货上架
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse AutoShelves(PurchaseOperateDto request, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inbound/Purchase/AutoShelves", query, request);
        }

        /// <summary>
        /// 修改业务类型（指定上下行）
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<bool> UpdatePurchaseBusinessTypeBySysId(string sysId, string businessType, Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysId, businessType, warehouseSysId };
            return ApiClient.Post<bool>(PublicConst.WmsApiUrl, "/Inbound/Purchase/UpdatePurchaseBusinessTypeBySysId", query);
        }
        
        #endregion

        #region 入库

        /// <summary>
        /// 根据入库单SysId获取数据
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public ApiResponse<ReceiptOperationDto> GetReceiptOperationByOrderNumber(string orderNumber, string currentUserName, int currentUserId, Guid currentWarehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new
            {
                orderNumber,
                currentUserName,
                currentUserId,
                currentWarehouseSysId,
            };
            return ApiClient.Post<ReceiptOperationDto>(PublicConst.WmsApiUrl, "/Inbound/Receipt/GetReceiptOperationByOrderNumber", query);
        }

        /// <summary>
        /// 保存入库单明细
        /// </summary>
        /// <param name="receiptOperationDto"></param>
        /// <returns></returns>
        public ApiResponse<string> SaveReceiptOperation(ReceiptOperationDto receiptOperationDto)
        {
            var query = new CoreQuery();
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "/Inbound/Receipt/SaveReceiptOperation", query, receiptOperationDto);
        }

        /// <summary>
        /// 根据采购单号生成入库单表头
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public ApiResponse<ReceiptOperationDto> CreateReceiptByPoOrder(string orderNumber,string currentUserName, int currentUserId,Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new
            {
                orderNumber,
                currentUserName,
                currentUserId,
                warehouseSysId
            };
            return ApiClient.Post<ReceiptOperationDto>(PublicConst.WmsApiUrl, "/Inbound/Receipt/CreateReceiptByPoOrder", query);
        }

        /// <summary>
        /// 更新采购状态
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ApiResponse<string> UpdateReceiptStatus(Guid sysId, ReceiptStatus status, string currentUserName, int currentUserId, Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysId, status, currentUserName, currentUserId, warehouseSysId };
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "/Inbound/Receipt/UpdateReceiptStatus", query);
        }

        /// <summary>
        /// 获取收货单列表
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<ReceiptListDto>> GetReceiptList(CoreQuery query, ReceiptQuery receiptQuery)
        {
            return ApiClient.Post<Pages<ReceiptListDto>>(PublicConst.WMSReportUrl, "/OtherRead/GetReceiptList", query, receiptQuery);
        }

        /// <summary>
        /// 获取收货单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<ReceiptViewDto> GetReceiptViewById(CoreQuery query, string sysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<ReceiptViewDto>(PublicConst.WMSReportUrl, "OtherRead/GetReceiptViewById", query);
        }

        /// <summary>
        /// 收货清单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<List<ReceiptDetailViewDto>> GetReceiptDetailViewList(CoreQuery query, string sysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Post<List<ReceiptDetailViewDto>>(PublicConst.WMSReportUrl, "OtherRead/GetReceiptDetailViewList", query);
        }

        /// <summary>
        /// 收货批次清单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ApiResponse<List<ReceiptDetailViewDto>> GetReceiptDetailLotViewList(CoreQuery query, string sysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Post<List<ReceiptDetailViewDto>>(PublicConst.WMSReportUrl, "OtherRead/GetReceiptDetailLotViewList", query);
        }

        /// <summary>
        /// 获取ReceiptDetail Sku  UPC 是null 的
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        public ApiResponse<List<ReceiptDetailSkuDto>> GetPurchaseDetailSkuByUpcIsNull(Guid purchaseSysId,Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { purchaseSysId, warehouseSysId }; 
            return ApiClient.Post<List<ReceiptDetailSkuDto>>(PublicConst.WmsApiUrl, "/Inbound/Purchase/GetPurchaseDetailSkuByUpcIsNull", query);
        }

        /// <summary>
        /// 保存SKU 属性
        /// </summary>
        /// <param name="purchaseDetailSkuDto"></param>
        public ApiResponse<string> SavePurchaseDetailSkuStyle(PurchaseDetailSkuDto purchaseDetailSkuDto)
        {
            var query = new CoreQuery();
        
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "/Inbound/Purchase/SavePurchaseDetailSkuStyle", query, purchaseDetailSkuDto);
        }

        /// <summary>
        /// 入库单生成质检单 属性
        /// </summary>
        /// <param name="purchaseQcDto"></param>
        public ApiResponse<bool> GenerateQcOrderByPurchase(PurchaseQcDto purchaseQcDto)
        {
            var query = new CoreQuery();

            return ApiClient.Post<bool>(PublicConst.WmsApiUrl, "/Inbound/Purchase/GenerateQcOrderByPurchase", query, purchaseQcDto);
        }

        /// <summary>
        /// 取消收货（入库单整单取消）
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<CommonResponse> CancelReceiptByPurchase(ReceiptCancelDto request, CoreQuery query)
        {
            return ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "Inbound/Receipt/CancelReceiptByPurchase", query, request);
        }

        #region 批次采集
        /// <summary>
        /// 批次采集时获取收货清单明细`
        /// </summary>
        /// <param name="query"></param>
        /// <param name="receiptSysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public ApiResponse<List<ReceiptDetailViewDto>> GetReceiptDetailViewListByCollectionLot(CoreQuery query, string receiptSysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { receiptSysId, warehouseSysId };
            return ApiClient.Post<List<ReceiptDetailViewDto>>(PublicConst.WMSReportUrl, "OtherRead/GetReceiptDetailViewListByCollectionLot", query);
        }

        /// <summary>
        /// 根据商品获取批次采集相关信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<ReceiptCollectionLotViewDto> GetReceiptDetailCollectionLotViewList(ReceiptCollectionLotQuery request, CoreQuery query)
        {
            return ApiClient.Post<ReceiptCollectionLotViewDto>(PublicConst.WmsApiUrl, "Inbound/Receipt/GetReceiptDetailCollectionLotViewList", query, request);
        }

        /// <summary>
        /// 保存批次采集
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<ReceiptDetailResponseDto> SaveReceiptDetailLot(ReceiptCollectionLotDto request, CoreQuery query)
        {
            return ApiClient.Post<ReceiptDetailResponseDto>(PublicConst.WmsApiUrl, "Inbound/Receipt/SaveReceiptDetailLot", query, request);
        }
        #endregion

        #endregion

        #region 上架
        /// <summary>
        /// 检查商品是否存在于收货明细中
        /// </summary>
        public ApiResponse<RFCommResult> CheckReceiptDetailSku(ScanShelvesDto scanShelvesDto)
        {
            return ApiClient.Post<RFCommResult>(PublicConst.WmsApiUrl, "/Shelves/CheckReceiptDetailSku", new CoreQuery(), scanShelvesDto);
        }

        /// <summary>
        /// 上架
        /// </summary>
        /// <param name="scanShelvesDto"></param>
        /// <returns></returns>
        public ApiResponse<RFCommResult> ScanShelves(ScanShelvesDto scanShelvesDto)
        {
            return ApiClient.Post<RFCommResult>(PublicConst.WmsApiUrl, "/Shelves/ScanShelves", new CoreQuery(), scanShelvesDto);
        }

        /// <summary>
        /// 自动上架
        /// </summary>
        /// <param name="scanShelvesDto"></param>
        /// <returns></returns>
        public ApiResponse<RFCommResult> AutoShelves(ScanShelvesDto scanShelvesDto)
        {
            return ApiClient.Post<RFCommResult>(PublicConst.WmsApiUrl, "/Shelves/AutoShelves", new CoreQuery(), scanShelvesDto);
        }

        

        /// <summary>
        /// 获取推荐货位
        /// </summary>
        /// <param name="shelvesQuery"></param>
        /// <returns></returns>
        public ApiResponse<string> GetAdviceToLoc(RFShelvesQuery shelvesQuery)
        {
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "/Shelves/GetAdviceToLoc", new CoreQuery(), shelvesQuery);
        }

        /// <summary>
        /// 查询库存
        /// </summary>
        /// <param name="shelvesQuery"></param>
        /// <returns></returns>
        public ApiResponse<List<RFInventoryListDto>> GetInventoryList(RFInventoryQuery inventoryQuery)
        {
            return ApiClient.Post<List<RFInventoryListDto>>(PublicConst.WmsApiUrl, "/Shelves/GetInventoryList", new CoreQuery(), inventoryQuery);
        }

        #endregion

        #region 批量采购

        /// <summary>
        /// 采购单收货
        /// </summary>
        /// <param name="query"></param>AppointBatchNumber
        /// <param name="purchaseBatchDto"></param>
        /// <returns></returns>
        public ApiResponse SaveBatchPurchaseAndReceipt(CoreQuery query, PurchaseBatchDto purchaseBatchDto)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/Inbound/Purchase/SaveBatchPurchaseAndReceipt", new CoreQuery(), purchaseBatchDto);
        }

        /// <summary>
        /// 获取供应商
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<List<SelectItem>> SelectItemVendor(CoreQuery query)
        {
            return ApiClient.Get<List<SelectItem>>(PublicConst.WmsApiUrl, "/Vendor/GetSelectVendor", query);
        }

        /// <summary>
        /// 取消上架
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancelShelvesDto"></param>
        /// <returns></returns>
        public ApiResponse<CommonResponse> CancelShelves(CoreQuery query, CancelShelvesDto cancelShelvesDto)
        {
            return ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "/Shelves/CancelShelves", query, cancelShelvesDto);
        }
        #endregion

        #region 领料分拣
        /// <summary>
        /// 领料分拣
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<CommonResponse> PickingMaterial(PickingMaterialDto request, CoreQuery query)
        {
            return ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "Inbound/Receipt/PickingMaterial", query, request);
        }

        /// <summary>
        /// 获取领料分拣记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<PickingMaterialListDto>> GetPickingMaterialList(PickingMaterialQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<PickingMaterialListDto>>(PublicConst.WmsApiUrl, "Inbound/Receipt/GetPickingMaterialList", query, request);
        }
        #endregion

        /// <summary>
        /// 校验扫描的（红卡）SN在DB中是否重复
        /// </summary>
        /// <param name="snList"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public ApiResponse<CheckDuplicateSNDto> CheckDuplicateSN(List<string> snList, string type, Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { warehouseSysId , type };
            return ApiClient.Post<CheckDuplicateSNDto>(PublicConst.WmsApiUrl, "Inbound/Receipt/CheckDuplicateSN", query, snList);
        }
    }
}