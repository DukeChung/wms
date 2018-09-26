using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBK.WMS.Portal.Services
{
    public class VASApiClient
    {
        private static readonly VASApiClient instance = new VASApiClient();

        private VASApiClient() { }

        public static VASApiClient GetInstance()
        {
            return instance;
        }

        #region 组装单

        public ApiResponse<Pages<AssemblyListDto>> GetAssemblyList(CoreQuery query, AssemblyQuery assemblyQuery)
        {
            return ApiClient.Post<Pages<AssemblyListDto>>(PublicConst.WmsApiUrl, "/VAS/Assembly/GetAssemblyList", query, assemblyQuery);
        }

        public ApiResponse<AssemblyViewDto> GetAssemblyViewDtoById(CoreQuery query, Guid sysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<AssemblyViewDto>(PublicConst.WmsApiUrl, "/VAS/Assembly/GetAssemblyViewDtoById", query);
        }

        public ApiResponse UpdateAssemblyStatus(CoreQuery query, Guid sysId, AssemblyStatus status, int currentUserId, string currentUserName, Guid warehouseSysId)
        {
            query.ParmsObj = new { sysId, status, currentUserId, currentUserName, warehouseSysId };
            return ApiClient.Get(PublicConst.WmsApiUrl, "/VAS/Assembly/UpdateAssemblyStatus", query);
        }

        public ApiResponse CancelAssemblyPicking(CoreQuery query, Guid sysId, int currentUserId, string currentUserName, Guid warehouseSysId)
        {
            query.ParmsObj = new { sysId, currentUserId, currentUserName, warehouseSysId };
            return ApiClient.Get(PublicConst.WmsApiUrl, "/VAS/Assembly/CancelAssemblyPicking", query);
        }

        public ApiResponse FinishAssemblyOrder(CoreQuery query, AssemblyFinishDto assemblyFinishDto)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/VAS/Assembly/FinishAssemblyOrder", query, assemblyFinishDto);
        }

        public ApiResponse<Pages<AssemblySkuDto>> GetSkuListForAssembly(AssemblySkuQuery request , CoreQuery query)
        {
            return ApiClient.Post<Pages<AssemblySkuDto>>(PublicConst.WmsApiUrl, "/VAS/Assembly/GetSkuListForAssembly", query, request);
        }

        public ApiResponse AddAssembly(AddAssemblyDto request, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/VAS/Assembly/AddAssembly", query, request);
        }

        public ApiResponse<Pages<AssemblyWeightSkuDto>> GetWeighSkuListForAssembly(AssemblyWeightSkuQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<AssemblyWeightSkuDto>>(PublicConst.WmsApiUrl, "/VAS/Assembly/GetWeighSkuListForAssembly", query, request);
        }

        public ApiResponse SaveAssemblySkuWeight(AssemblyWeightSkuRequest request, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/VAS/Assembly/SaveAssemblySkuWeight", query, request);
        }

        public ApiResponse<List<RFInventoryListDto>> GetInventoryList(CoreQuery query, RFInventoryQuery request)
        {
            return ApiClient.Post<List<RFInventoryListDto>>(PublicConst.WmsApiUrl, "/Shelves/GetInventoryList", query, request);
        }

        public ApiResponse<RFCommResult> LocIsExist(CoreQuery query, LocationQuery request)
        {
            return ApiClient.Post<RFCommResult>(PublicConst.WmsApiUrl, "/Base/LocIsExist", query, request);
        }

        public ApiResponse<RFCommResult> AssemblyScanPickDetail(CoreQuery query, RFAssemblyPickDetailDto request)
        {
            return ApiClient.Post<RFCommResult>(PublicConst.WmsApiUrl, "/PickDetail/AssemblyScanPickDetail", query, request);
        }

        public ApiResponse<RFCommResult> AssemblyScanShelves(CoreQuery query, RFAssemblyScanShelvesDto request)
        {
            return ApiClient.Post<RFCommResult>(PublicConst.WmsApiUrl, "/Shelves/AssemblyScanShelves", query, request);
        }

        #endregion

        #region 质检单
        public ApiResponse<Pages<QualityControlListDto>> GetQualityControlList(CoreQuery query, QualityControlQuery qualityControlQuery)
        {
            return ApiClient.Post<Pages<QualityControlListDto>>(PublicConst.WmsApiUrl, "/VAS/QualityControl/GetQualityControlList", query, qualityControlQuery);
        }

        public ApiResponse DeleteQualityControl(CoreQuery query, List<Guid> sysIds, Guid warehouseSysId)
        {
            query.ParmsObj = new { warehouseSysId };
            return ApiClient.Post(PublicConst.WmsApiUrl, "/VAS/QualityControl/DeleteQualityControl", query, sysIds);
        }

        public ApiResponse<QualityControlDto> GetQualityControlViewDto(CoreQuery query, Guid sysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<QualityControlDto>(PublicConst.WmsApiUrl, "/VAS/QualityControl/GetQualityControlViewDto", query);
        }

        public ApiResponse<Pages<DocDetailDto>> GetDocDetails(CoreQuery query, DocDetailQuery docDetailQuery)
        {
            return ApiClient.Post<Pages<DocDetailDto>>(PublicConst.WmsApiUrl, "/VAS/QualityControl/GetDocDetails", query, docDetailQuery);
        }

        public ApiResponse<Pages<QualityControlDetailDto>> GetQCDetails(CoreQuery query, QCDetailQuery qcDetailQuery)
        {
            return ApiClient.Post<Pages<QualityControlDetailDto>>(PublicConst.WmsApiUrl, "/VAS/QualityControl/GetQCDetails", query, qcDetailQuery);
        }

        public ApiResponse SaveQualityControl(CoreQuery query, SaveQualityControlDto saveQualityControlDto)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/VAS/QualityControl/SaveQualityControl", query, saveQualityControlDto);
        }

        public ApiResponse FinishQualityControl(CoreQuery query, FinishQualityControlDto finishQualityControlDto)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/VAS/QualityControl/FinishQualityControl", query, finishQualityControlDto);
        }

        public ApiResponse<AdjustmentDto> GetAdjustmentDto(CoreQuery query, CreateAdjustmentDto createAdjustmentDto)
        {
            return ApiClient.Post<AdjustmentDto>(PublicConst.WmsApiUrl, "/VAS/QualityControl/GetAdjustmentDto", query, createAdjustmentDto);
        }
        #endregion

        #region 商品外借单

        /// <summary>
        /// 分页获取商品外借单
        /// </summary>
        /// <param name="adjustmentQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<SkuBorrowListDto>> GetSkuBorrowListByPage(SkuBorrowQuery skuborrowQuery, CoreQuery query)
        {
            //return ApiClient.Post<Pages<SkuBorrowListDto>>(PublicConst.WmsApiUrl, "/VAS/SkuBorrow/GetSkuBorrowListByPage", query, skuborrowQuery);
            return ApiClient.Post<Pages<SkuBorrowListDto>>(PublicConst.WMSReportUrl, "/Report/GetSkuBorrowListByPage", query, skuborrowQuery);
        }

        /// <summary>
        ///  获取商品库存分页
        /// </summary>
        /// <param name="adjustmentQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<SkuInvLotLocLpnDto>> GetSkuListByPage(SkuInvLotLocLpnQuery adjustmentQuery, CoreQuery query)
        {
            return ApiClient.Post<Pages<SkuInvLotLocLpnDto>>(PublicConst.WmsApiUrl, "/VAS/SkuBorrow/GetSkuInventoryList", query, adjustmentQuery);
        }

        /// <summary>
        /// 添加商品外借单
        /// </summary>
        /// <param name="skuborrow"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse AddSkuBorrow(SkuBorrowDto skuborrow, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/VAS/SkuBorrow/AddSkuBorrow", query, skuborrow);
        }

        /// <summary>
        /// 获取商品外借单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<SkuBorrowViewDto> GetSkuBorrowBySysId(Guid sysId, Guid WareHouseSysId, CoreQuery query)
        {
            query.ParmsObj = new { SysId = sysId, WareHouseSysId = WareHouseSysId };
            return ApiClient.Get<SkuBorrowViewDto>(PublicConst.WmsApiUrl, "/VAS/SkuBorrow/GetSkuBorrowBySysId", query);
        }

        /// <summary>
        /// 获取商品外借单
        /// </summary>
        /// <param name="skuBorrowOrder"></param>
        /// <param name="warehouseSysId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<SkuBorrowViewDto> GetSkuBorrowByOrder(string skuBorrowOrder,Guid warehouseSysId, CoreQuery query)
        {
            query.ParmsObj = new { BorrowOrder = skuBorrowOrder, WareHouseSysId = warehouseSysId };
            return ApiClient.Get<SkuBorrowViewDto>(PublicConst.WmsApiUrl, "/VAS/SkuBorrow/GetSkuBorrowByOrder", query);
        }

        /// <summary>
        /// 删除商品外界单商品
        /// </summary>
        /// <param name="sysIds"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse DeleteSkuBorrowSkus(List<Guid> sysIds, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/VAS/SkuBorrow/DeleteSkuBorrowSkus", query, sysIds);
        }

        /// <summary>
        /// 编辑商品外接单
        /// </summary>
        /// <param name="skuborrow"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse UpdateSkuBorrow(SkuBorrowDto skuborrow, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/VAS/SkuBorrow/UpdateSkuBorrow", query, skuborrow);
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="skuborrow"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse Audit(SkuBorrowDto skuborrow, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/VAS/SkuBorrow/Audit", query, skuborrow);
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="skuborrow"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse Void(SkuBorrowAuditDto skuborrow, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/VAS/SkuBorrow/Void", query, skuborrow);
        }

        #endregion


        #region 工单管理
        /// <summary>
        /// 分页查询工单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<WorkListDto>> GetWorkByPage(WorkQueryDto request)
        {
            var query = new CoreQuery();
            return ApiClient.Post<Pages<WorkListDto>>(PublicConst.WmsApiUrl, "/WorkManger/GetWorkByPage", query, request);
        }

        /// <summary>
        /// 获取工单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<WorkDetailDto> GetWorkBySysId(Guid sysId, Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysId = sysId, warehouseSysId = warehouseSysId };
            return ApiClient.Get<WorkDetailDto>(PublicConst.WmsApiUrl, "/WorkManger/GetWorkBySysId", query);
        }

        /// <summary>
        /// 编辑工单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse UpdateWorkInfo(WorkUpdateDto request, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/WorkManger/UpdateWorkInfo", query, request);
        }


        /// <summary>
        /// 作废工单
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        public ApiResponse CancelWork(CoreQuery query, CancelWorkDto dto)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "/WorkManger/CancelWork", query, dto);
        }

        #endregion
    }
}