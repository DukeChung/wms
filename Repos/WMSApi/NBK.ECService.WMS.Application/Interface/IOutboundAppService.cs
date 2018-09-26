using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using NBK.ECService.WMS.DTO.ThirdParty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IOutboundAppService : IApplicationService
    {

        List<ScanDeliveryDto> GetDeliveryBoxByOrderNumber(string type, string orderNumber, Guid wareHouseSysId);

        void SaveDeliveryByVanningSysId(List<Guid> vanningSysIds, string currentUserName, int currentUserId, Guid wareHouseSysId);

        string BatchOutboundByFIFO(OutboundBatchDto outboundBatchDto);


        /// <summary>
        /// 根据条件获取出库单信息
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        OutboundViewDto GetOutboundOrderByOrderId(OutboundQuery outboundQuery);

        /// <summary>
        /// 快速发货(接收到数据发送到消息队列)
        /// </summary>
        CommonResponse OutboundQuickDeliverySendMQ(OutboundQuickDeliveryDto outboundQuickDeliveryDto);

        /// <summary>
        /// 一键发货
        /// </summary>
        /// <param name="outboundQuickDeliveryDto"></param>
        /// <returns></returns>
        CommonResponse OutboundQuickDelivery(OutboundQuickDeliveryDto outboundQuickDeliveryDto);

        /// <summary>
        /// 通过MQ异步创建出库单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        CommonResponse CreateOutboundByMQ(MQProcessDto<ThirdPartyOutboundDto> request);

        #region 分配发货
        /// <summary>
        /// 分配发货
        /// </summary>
        /// <param name="outboundAllocationDeliveryDto"></param>
        /// <returns></returns>
        CommonResponse OutboundAllocationDelivery(OutboundAllocationDeliveryDto outboundAllocationDeliveryDto);

        /// <summary>
        /// 分配发货检查差异
        /// </summary>
        /// <param name="outboundAllocationDeliveryDto"></param>
        /// <returns></returns>
        CommonResponse CheckOutboundAllocationDelivery(OutboundAllocationDeliveryDto outboundAllocationDeliveryDto);

        /// <summary>
        /// 获取部分发货商品明细
        /// </summary>
        /// <param name="outboundAllocationDeliveryDto"></param>
        /// <returns></returns>
        List<PartShipmentDetailDto> GetPartShipmentSkuList(OutboundAllocationDeliveryDto outboundAllocationDeliveryDto);

        /// <summary>
        /// 更新出库明细备注
        /// </summary>
        /// <param name="partShipmentMemoDto"></param>
        /// <returns></returns>
        CommonResponse SavePartShipmentMemo(PartShipmentMemoDto partShipmentMemoDto);
        #endregion

        /// <summary>
        /// 作废出库单
        /// </summary>
        /// <param name="outboundDto"></param>
        /// <returns></returns>
        bool ObsoleteOutbound(OutboundOperateDto outboundDto);

        /// <summary>
        /// 退货入库
        /// </summary>
        /// <param name="outboundDto"></param>
        void OutboundReturn(OutboundOperateDto outboundDto);

        /// <summary>
        /// 部分退货入库
        /// </summary>
        /// <param name="outboundDto"></param>
        void OutboundPartReturn(OutboundPartReturnDto outboundDto);

        /// <summary>
        /// 取消发货
        /// </summary>
        /// <param name="outboundDto"></param>
        void OutboundCancel(OutboundOperateDto outboundDto);

        /// <summary>
        /// 获取出库单预包装差异
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        OutboundPrePackDiffDto GetOutboundPrePackDiff(Guid outboundSysId, Guid warehouseSysId);

        /// <summary>
        /// 获取出库单散货箱差异
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        OutboundPrePackDiffDto GetOutboundPreBulkPackDiff(Guid outboundSysId);

        /// <summary>
        ///绑定预包装单
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        bool BindPrePackOrder(OutboundBindQuery dto);

        /// <summary>
        /// 解绑预包装单
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        bool UnBindPrePackOrder(OutboundBindQuery dto);

        /// <summary>
        /// 打印交接单号
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        TMSBoxNumberDto AddTMSBoxNumber(BatchTMSBoxNumberDto dto);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        CommonResponse CreateTMSBoxCount(BatchTMSBoxNumberDto dto);

        /// <summary>
        /// 获取库存不足商品
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        List<InsufficientStockSkuListDto> GetInsufficientStockSkuList(OutboundAllocationDeliveryDto dto);

        /// <summary>
        /// 增加异常记录数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        CommonResponse AddOutboundExceptionService(AddOutboundExceptionDto dto);

        /// <summary>
        /// 删除异常数据记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="warehouseSysId"></param>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        CommonResponse DeleteOutboundException(List<Guid> request, Guid warehouseSysId, Guid outboundSysId);

        void CancelOutboundReturnByPurchase(PurchaseForReturnDto request);

        OutboundReturnDto AddOutboundReturnQtyByPurchase(OutboundReturnDto request);
    }
}
