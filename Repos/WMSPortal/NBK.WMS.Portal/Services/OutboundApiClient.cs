using System.Collections.Generic;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using NBK.ECService.WMS.Utility;
using System;

namespace NBK.WMS.Portal.Services
{
    public class OutboundApiClient
    {

        private static readonly OutboundApiClient instance = new OutboundApiClient();

        private OutboundApiClient()
        {
        }

        public static OutboundApiClient GetInstance()
        {
            return instance;
        }


        #region 拣货

        /// <summary>
        /// 获取拣货列表
        /// </summary> 
        /// <param name="pickDetailQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<PickDetailListDto>> GetPickDetailListDto(PickDetailQuery pickDetailQuery)
        {
            var query = new CoreQuery();
            return ApiClient.Post<Pages<PickDetailListDto>>(PublicConst.WMSReportUrl, "OtherRead/GetPickDetailListDto", query, pickDetailQuery);
        }

        /// <summary>
        /// 获取拣货列表
        /// </summary> 
        /// <param name="pickDetailQuery"></param>
        /// <returns></returns>
        public ApiResponse<Pages<PickOutboundListDto>> GetPickOutboundListDto(PickDetailQuery pickDetailQuery)
        {
            var query = new CoreQuery();
            return ApiClient.Post<Pages<PickOutboundListDto>>(PublicConst.WMSReportUrl, "OtherRead/GetPickOutboundListDto", query, pickDetailQuery);
        }


        /// <summary>
        /// 生成拣货明细
        /// </summary>
        /// <param name="createPickDetailRuleDto"></param>
        /// <returns></returns>
        public ApiResponse<List<string>> GeneratePickDetailByPickRule(CreatePickDetailRuleDto createPickDetailRuleDto)
        {
            var query = new CoreQuery();
            return ApiClient.Post<List<string>>(PublicConst.WmsApiUrl, "/Outbound/PickDetail/GeneratePickDetailByPickRule", query, createPickDetailRuleDto);
        }

        /// <summary>
        /// 取消拣货 
        /// </summary>
        /// <returns></returns>
        public ApiResponse<string> CancelPickDetail(CancelPickDetailDto cancelPickDetailDto)
        {
            var query = new CoreQuery();
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "/Outbound/PickDetail/CancelPickDetail", query, cancelPickDetailDto);
        }

        /// <summary>
        /// 取消拣货数量
        /// </summary>
        /// <param name="cancelPickQtyDto"></param>
        /// <returns></returns>
        public ApiResponse<string> CancelPickQty(CancelPickQtyDto cancelPickQtyDto)
        {
            var query = new CoreQuery();
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "/Outbound/PickDetail/CancelPickQty", query, cancelPickQtyDto);
        }

        /// <summary>
        /// 拣货
        /// </summary>
        /// <param name="pickingOperationDto"></param>
        /// <returns></returns>
        public ApiResponse SavePickingOperation(PickingOperationDto pickingOperationDto)
        {
            var query = new CoreQuery();
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "/Outbound/PickDetail/SavePickingOperation", query, pickingOperationDto);
        }

        /// <summary>
        /// 获取拣货单明细
        /// </summary>
        /// <param name="pickingOperationQuery"></param>
        /// <returns></returns>
        public ApiResponse<List<PickingOperationDetail>> GetPickingOperationDetails(PickingOperationQuery pickingOperationQuery)
        {
            var query = new CoreQuery();
            return ApiClient.Post<List<PickingOperationDetail>>(PublicConst.WmsApiUrl, "/Outbound/PickDetail/GetPickingOperationDetails", query, pickingOperationQuery);
        }

        #endregion

        #region 出库管理
        /// <summary>
        /// 分页查询出库单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<Pages<OutboundListDto>> GetOutboundByPage(OutboundQuery request)
        {
            var query = new CoreQuery();
            return ApiClient.Post<Pages<OutboundListDto>>(PublicConst.WMSReportUrl, "/OtherRead/GetOutboundByPage", query, request);
        }

        public ApiResponse<OutboundViewDto> GetOutboundBySysId(Guid sysId, Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<OutboundViewDto>(PublicConst.WMSReportUrl, "OtherRead/GetOutboundBySysId", query);
        }

        /// <summary>
        /// 分页获取获取出库单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public ApiResponse<Pages<OutboundExceptionDto>> GetOutboundDetailList(OutboundExceptionQueryDto dto)
        {
            var query = new CoreQuery();
            return ApiClient.Post<Pages<OutboundExceptionDto>>(PublicConst.WMSReportUrl, "OtherRead/GetOutboundDetailList", query, dto);
        }

        /// <summary>
        /// 根据出库单获取异常记录
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public ApiResponse<List<OutboundExceptionDtoList>> GetOutbooundExceptionData(Guid sysId, Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { sysId, warehouseSysId };
            return ApiClient.Get<List<OutboundExceptionDtoList>>(PublicConst.WMSReportUrl, "OtherRead/GetOutbooundExceptionData", query);
        }

        /// <summary>
        /// 删除异常记录数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public ApiResponse<CommonResponse> DeleteOutboundException(List<Guid> sysIds, Guid warehouseSysId, Guid outboundSysId, CoreQuery query)
        {
            query.ParmsObj = new { warehouseSysId, outboundSysId };
            return ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "Outbound/DeleteOutboundException", query, sysIds);
        }

        /// <summary>
        /// 新增异常登记数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<CommonResponse> AddOutboundExceptionService(AddOutboundExceptionDto request, CoreQuery query)
        {
            return ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "Outbound/AddOutboundExceptionService", query, request);
        }

        /// <summary>
        /// 作废出库单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<bool> ObsoleteOutbound(OutboundOperateDto request, CoreQuery query)
        {
            return ApiClient.Post<bool>(PublicConst.WmsApiUrl, "Outbound/ObsoleteOutbound", query, request);
        }

        /// <summary>
        /// 退货入库
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse OutboundReturn(OutboundOperateDto request, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "Outbound/OutboundReturn", query, request);
        }

        /// <summary>
        /// 部分退货入库
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse OutboundPartReturn(OutboundPartReturnDto request, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "Outbound/OutboundPartReturn", query, request);
        }

        /// <summary>
        /// 取消出库
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse OutboundCancel(OutboundOperateDto request, CoreQuery query)
        {
            return ApiClient.Post(PublicConst.WmsApiUrl, "Outbound/OutboundCancel", query, request);
        }

        /// <summary>
        /// 获取出库单预包装差异
        /// </summary>
        /// <param name="query"></param>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        public ApiResponse<OutboundPrePackDiffDto> GetOutboundPrePackDiff(CoreQuery query, Guid outboundSysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { outboundSysId };
            return ApiClient.Get<OutboundPrePackDiffDto>(PublicConst.WMSReportUrl, "OtherRead/GetOutboundPrePackDiff", query);
        }

        /// <summary>
        /// 获取出库单散货箱差异
        /// </summary>
        /// <param name="query"></param>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        public ApiResponse<OutboundPrePackDiffDto> GetOutboundPreBulkPackDiff(CoreQuery query, Guid outboundSysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { outboundSysId, warehouseSysId };
            return ApiClient.Get<OutboundPrePackDiffDto>(PublicConst.WMSReportUrl, "OtherRead/GetOutboundPreBulkPackDiff", query);
        }

        /// <summary>
        /// 获取出库单整件或者散件装箱数据
        /// </summary>
        /// <param name="query"></param>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        public ApiResponse<OutboundBoxListDto> GetOutboundBox(CoreQuery query, Guid outboundSysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { outboundSysId, warehouseSysId };
            return ApiClient.Get<OutboundBoxListDto>(PublicConst.WMSReportUrl, "OtherRead/GetOutboundBox", query);
        }
        #endregion

        #region 装箱

        public ApiResponse<Pages<VanningDto>> GetVanningList(VanningQueryDto vanningQueryDto)
        {
            var query = new CoreQuery();

            return ApiClient.Post<Pages<VanningDto>>(PublicConst.WmsApiUrl, "Outbound/Vanning/GetVanningList", query, vanningQueryDto);
        }

        /// <summary>
        /// 装箱业务
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public ApiResponse<VanningOperationDto> GetVanningOperationDtoByOrder(string orderNumber, Guid wareHouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new
            {
                orderNumber,
                wareHouseSysId
            };
            return ApiClient.Post<VanningOperationDto>(PublicConst.WmsApiUrl, "Outbound/Vanning/GetVanningOperationDtoByOrder", query);
        }

        /// <summary>
        /// 装箱保存
        /// </summary>
        /// <param name="vanningDetailOperationDto"></param>
        /// <returns></returns>
        public ApiResponse<VanningDetailDto> SaveVanningDetailOperationDto(List<VanningDetailOperationDto> vanningDetailOperationDto, string actionType, string currentUserName, int currentUserId, Guid wareHouseSysId)
        {
            var vanningOperationDto = new VanningOperationDto();
            vanningOperationDto.VanningDetailOperationDto = vanningDetailOperationDto;
            vanningOperationDto.ActionType = actionType;
            vanningOperationDto.CurrentUserId = currentUserId;
            vanningOperationDto.CurrentUserName = currentUserName;
            vanningOperationDto.WarehouseSysId = wareHouseSysId;
            var query = new CoreQuery();
            return ApiClient.Post<VanningDetailDto>(PublicConst.WmsApiUrl, "Outbound/Vanning/SaveVanningDetailOperationDto", query, vanningOperationDto);
        }

        /// <summary>
        /// 物流交接单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<Pages<HandoverGroupDto>> GetHandoverGroupByPage(HandoverGroupQuery request, CoreQuery query)
        {
            return ApiClient.Post<Pages<HandoverGroupDto>>(PublicConst.WmsApiUrl, "Outbound/Vanning/GetHandoverGroupByPage", query, request);
        }

        public ApiResponse<HandoverGroupDto> GetHandoverGroupByOrder(string handoverGroupOrder, Guid wareHouseSysId, CoreQuery query)
        {
            query.ParmsObj = new { handoverGroupOrder, wareHouseSysId };
            return ApiClient.Get<HandoverGroupDto>(PublicConst.WmsApiUrl, "/Outbound/Vanning/GetHandoverGroupByOrder", query);
        }

        public ApiResponse<VanningViewDto> GetVanningViewById(CoreQuery query, VanningViewQuery vanningViewQuery)
        {
            return ApiClient.Post<VanningViewDto>(PublicConst.WmsApiUrl, "Outbound/Vanning/GetVanningViewById", query, vanningViewQuery);
        }

        public ApiResponse<Guid?> GetVanningSysIdByVanningDetailSysId(CoreQuery query, Guid vanningDetailSysId, Guid warehouseSysId)
        {
            query.ParmsObj = new { vanningDetailSysId, warehouseSysId };
            return ApiClient.Get<Guid?>(PublicConst.WmsApiUrl, "/Outbound/Vanning/GetVanningSysIdByVanningDetailSysId", query);
        }

        /// <summary>
        /// 取消装箱
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<CommonResponse> CancelVanning(VanningCancelDto request, CoreQuery query)
        {
            return ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "/Outbound/Vanning/CancelVanning", query, request);
        }

        #endregion

        #region 发货管理
        public ApiResponse<List<ScanDeliveryDto>> GetDeliveryBoxByOrderNumber(string type, string orderNumber, Guid wareHouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { type, orderNumber, wareHouseSysId };
            return ApiClient.Post<List<ScanDeliveryDto>>(PublicConst.WmsApiUrl, "Outbound/GetDeliveryBoxByOrderNumber", query);
        }

        /// <summary>
        /// 发货 
        /// </summary>
        /// <param name="vanningSysId"></param>
        /// <returns></returns>
        public ApiResponse<string> SaveDeliveryByVanningSysId(List<Guid> vanningSysIds, string currentUserName, int currentUserId, Guid wareHouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new
            {
                currentUserName,
                currentUserId,
                wareHouseSysId
            };
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "Outbound/SaveDeliveryByVanningSysId", query, vanningSysIds);
        }
        #endregion

        #region 批量出库

        /// <summary>
        /// 批量出库
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<string> BatchOutboundCreate(OutboundBatchDto request, CoreQuery query)
        {
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "/Outbound/BatchOutboundByFIFO", query, request);
        }

        #endregion

        #region 一键出库

        public ApiResponse<CommonResponse> OutboundQuickDelivery(OutboundQuickDeliveryDto request, CoreQuery query)
        {
            return ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "/Outbound/OutboundQuickDelivery", query, request);
        }
        #endregion

        #region 分配发货
        /// <summary>
        /// 分配发货
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<CommonResponse> OutboundAllocationDelivery(OutboundAllocationDeliveryDto request, CoreQuery query)
        {
            return ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "/Outbound/OutboundAllocationDelivery", query, request);
        }

        /// <summary>
        /// 分配发货检查差异
        /// </summary>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<CommonResponse> CheckOutboundAllocationDelivery(CoreQuery query, OutboundAllocationDeliveryDto request)
        {
            return ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "/Outbound/CheckOutboundAllocationDelivery", query, request);
        }

        /// <summary>
        /// 获取部分发货商品明细
        /// </summary>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<List<PartShipmentDetailDto>> GetPartShipmentSkuList(CoreQuery query, OutboundAllocationDeliveryDto request)
        {
            return ApiClient.Post<List<PartShipmentDetailDto>>(PublicConst.WmsApiUrl, "/Outbound/GetPartShipmentSkuList", query, request);
        }

        /// <summary>
        /// 更新出库明细备注
        /// </summary>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<CommonResponse> SavePartShipmentMemo(CoreQuery query, PartShipmentMemoDto request)
        {
            return ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "/Outbound/SavePartShipmentMemo", query, request);
        }
        #endregion

        #region 绑定预包装单和解绑
        /// <summary>
        /// 绑定预包装单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<bool> BindPrePackOrder(OutboundBindQuery request, CoreQuery query)
        {
            return ApiClient.Post<bool>(PublicConst.WmsApiUrl, "Outbound/BindPrePackOrder", query, request);
        }

        /// <summary>
        /// 解绑预包装单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<bool> UnBindPrePackOrder(OutboundBindQuery request, CoreQuery query)
        {
            return ApiClient.Post<bool>(PublicConst.WmsApiUrl, "Outbound/UnBindPrePackOrder", query, request);
        }
        #endregion

        #region 打印交接单号
        /// <summary>
        /// 打印交接单号
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ApiResponse<TMSBoxNumberDto> AddTMSBoxNumber(BatchTMSBoxNumberDto dto, CoreQuery query)
        {
            return ApiClient.Post<TMSBoxNumberDto>(PublicConst.WmsApiUrl, "Outbound/AddTMSBoxNumber", query, dto);
        }
        #endregion

        #region 推送总箱数
        /// <summary>
        /// 打印交接单号
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ApiResponse<CommonResponse> CreateTMSBoxCount(BatchTMSBoxNumberDto dto, CoreQuery query)
        {
            return ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "Outbound/CreateTMSBoxCount", query, dto);
        }
        #endregion

        #region 获取库存不足商品
        /// <summary>
        /// 获取库存不足商品
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public ApiResponse<List<InsufficientStockSkuListDto>> GetInsufficientStockSkuList(OutboundAllocationDeliveryDto request, CoreQuery query)
        {
            return ApiClient.Post<List<InsufficientStockSkuListDto>>(PublicConst.WmsApiUrl, "/Outbound/GetInsufficientStockSkuList", query, request);
        }
        #endregion
    }
}