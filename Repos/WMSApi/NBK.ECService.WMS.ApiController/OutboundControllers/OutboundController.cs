using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMS.ApiController.OutboundControllers
{
    /// <summary>
    /// 出库管理
    /// </summary>
    [RoutePrefix("api/Outbound")]
    [AccessLog]
    public class OutboundController : AbpApiController
    {
        IOutboundAppService _outboundAppService;

        /// <summary>
        /// 系统构造
        /// </summary>
        public OutboundController(IOutboundAppService outboundAppService)
        {
            _outboundAppService = outboundAppService;
        }

        [HttpGet]
        public void OutboundAPI()
        {
        }

        ///// <summary>
        ///// 出库单管理
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[HttpPost, Route("GetOutboundByPage")]
        //public Pages<OutboundListDto> GetOutboundByPage(OutboundQuery request)
        //{
        //    return _outboundAppService.GetOutboundByPage(request);
        //}

        ///// <summary>
        ///// 获取出库单明细
        ///// </summary>
        ///// <param name="sysid"></param>
        ///// <returns></returns>
        //[HttpGet, Route("GetOutboundBySysId")]
        //public OutboundViewDto GetOutboundBySysId(Guid sysid)
        //{
        //    return _outboundAppService.GetOutboundBySysId(sysid);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="orderNumber"></param>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        [HttpPost, Route("GetDeliveryBoxByOrderNumber")]
        public List<ScanDeliveryDto> GetDeliveryBoxByOrderNumber(string type, string orderNumber, Guid wareHouseSysId)
        {
            return _outboundAppService.GetDeliveryBoxByOrderNumber(type, orderNumber, wareHouseSysId);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vanningSysIds"></param>
        /// <param name="currentUserName"></param>
        /// <param name="currentUserId"></param>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        [HttpPost, Route("SaveDeliveryByVanningSysId")]
        public void SaveDeliveryByVanningSysId(List<Guid> vanningSysIds, string currentUserName, int currentUserId, Guid wareHouseSysId)
        {
            _outboundAppService.SaveDeliveryByVanningSysId(vanningSysIds, currentUserName, currentUserId, wareHouseSysId);
        }

        /// <summary>
        /// 快进快出业务
        /// </summary>
        /// <param name="outboundBatchDto"></param>
        /// <returns></returns>
        [HttpPost, Route("BatchOutboundByFIFO")]
        public string BatchOutboundByFIFO(OutboundBatchDto outboundBatchDto)
        {
            return _outboundAppService.BatchOutboundByFIFO(outboundBatchDto);
        }

        /// <summary>
        /// 快速发货(系统调用)
        /// </summary>
        /// <param name="outboundQuickDeliveryDto"></param>
        /// <returns></returns>
        [HttpPost, Route("OutboundQuickDelivery")]
        public CommonResponse OutboundQuickDelivery(OutboundQuickDeliveryDto outboundQuickDeliveryDto)
        {
            return _outboundAppService.OutboundQuickDeliverySendMQ(outboundQuickDeliveryDto);
        }

        #region 分配发货
        /// <summary>
        /// 分配发货
        /// </summary>
        /// <param name="outboundAllocationDeliveryDto"></param>
        /// <returns></returns>
        [HttpPost, Route("OutboundAllocationDelivery")]
        public CommonResponse OutboundAllocationDelivery(OutboundAllocationDeliveryDto outboundAllocationDeliveryDto)
        {
            return _outboundAppService.OutboundAllocationDelivery(outboundAllocationDeliveryDto);
        }

        /// <summary>
        /// 分配发货检查差异
        /// </summary>
        /// <param name="outboundAllocationDeliveryDto"></param>
        /// <returns></returns>
        [HttpPost, Route("CheckOutboundAllocationDelivery")]
        public CommonResponse CheckOutboundAllocationDelivery(OutboundAllocationDeliveryDto outboundAllocationDeliveryDto)
        {
            return _outboundAppService.CheckOutboundAllocationDelivery(outboundAllocationDeliveryDto);
        }

        /// <summary>
        /// 获取部分发货商品明细
        /// </summary>
        /// <param name="outboundAllocationDeliveryDto"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPartShipmentSkuList")]
        public List<PartShipmentDetailDto> GetPartShipmentSkuList(OutboundAllocationDeliveryDto outboundAllocationDeliveryDto)
        {
            return _outboundAppService.GetPartShipmentSkuList(outboundAllocationDeliveryDto);
        }

        /// <summary>
        /// 更新出库明细备注
        /// </summary>
        /// <param name="partShipmentMemoDto"></param>
        /// <returns></returns>
        [HttpPost, Route("SavePartShipmentMemo")]
        public CommonResponse SavePartShipmentMemo(PartShipmentMemoDto partShipmentMemoDto)
        {
            return _outboundAppService.SavePartShipmentMemo(partShipmentMemoDto);
        }
        #endregion

        /// <summary>
        /// 作废出库单
        /// </summary>
        /// <param name="outboundDto"></param>
        /// <returns></returns>
        [HttpPost, Route("ObsoleteOutbound")]
        public bool ObsoleteOutbound(OutboundOperateDto outboundDto)
        {
            return _outboundAppService.ObsoleteOutbound(outboundDto);
        }

        /// <summary>
        /// 退货入库
        /// </summary>
        /// <param name="outboundDto"></param>
        [HttpPost, Route("OutboundReturn")]
        public void OutboundReturn(OutboundOperateDto outboundDto)
        {
            _outboundAppService.OutboundReturn(outboundDto);
        }

        /// <summary>
        /// 部分退货入库
        /// </summary>
        /// <param name="outboundDto"></param>
        [HttpPost, Route("OutboundPartReturn")]
        public void OutboundPartReturn(OutboundPartReturnDto outboundDto)
        {
            _outboundAppService.OutboundPartReturn(outboundDto);
        }

        /// <summary>
        /// 取消发货
        /// </summary>
        /// <param name="outboundDto"></param>
        [HttpPost, Route("OutboundCancel")]
        public void OutboundCancel(OutboundOperateDto outboundDto)
        {
            _outboundAppService.OutboundCancel(outboundDto);
        }

        /// <summary>
        /// 获取出库单预包装差异
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetOutboundPrePackDiff")]
        public OutboundPrePackDiffDto GetOutboundPrePackDiff(Guid outboundSysId, Guid warehouseSysId)
        {
            return _outboundAppService.GetOutboundPrePackDiff(outboundSysId, warehouseSysId);
        }

        /// <summary>
        /// 绑定出库单
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("BindPrePackOrder")]
        public bool BindPrePackOrder(OutboundBindQuery dto)
        {
            return _outboundAppService.BindPrePackOrder(dto);
        }

        /// <summary>
        /// 解绑出库单
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("UnBindPrePackOrder")]
        public bool UnBindPrePackOrder(OutboundBindQuery dto)
        {
            return _outboundAppService.UnBindPrePackOrder(dto);
        }


        /// <summary>
        /// 打印交接单号
        /// </summary>
        /// <param name="dto"></param>
        [HttpPost, Route("AddTMSBoxNumber")]
        public TMSBoxNumberDto AddTMSBoxNumber(BatchTMSBoxNumberDto dto)
        {
            return _outboundAppService.AddTMSBoxNumber(dto);
        }

        /// <summary>
        /// 给TMS推送总箱数
        /// </summary>
        /// <param name="dto"></param>
        [HttpPost, Route("CreateTMSBoxCount")]
        public CommonResponse CreateTMSBoxCount(BatchTMSBoxNumberDto dto)
        {
            return _outboundAppService.CreateTMSBoxCount(dto);
        }

        #region 获取库存不足商品
        /// <summary>
        /// 获取库存不足商品
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("GetInsufficientStockSkuList")]
        public List<InsufficientStockSkuListDto> GetInsufficientStockSkuList(OutboundAllocationDeliveryDto dto)
        {
            return _outboundAppService.GetInsufficientStockSkuList(dto);
        }
        #endregion

        /// <summary>
        ///增加异常记录数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("AddOutboundExceptionService")]
        public CommonResponse AddOutboundExceptionService(AddOutboundExceptionDto dto)
        {
            return _outboundAppService.AddOutboundExceptionService(dto);
        }

        /// <summary>
        /// 删除异常数据记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="warehouseSysId"></param>
        /// <param name="outboundSysId"></param>
        [HttpPost, Route("DeleteOutboundException")]
        public CommonResponse DeleteOutboundException(List<Guid> request, Guid warehouseSysId, Guid outboundSysId)
        {
            return _outboundAppService.DeleteOutboundException(request, warehouseSysId, outboundSysId);
        }

        /// <summary>
        /// 关闭或作为入库单时 ，回写更新outbound 对应的 returnqty
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("CancelOutboundReturnByPurchase")]
        public void CancelOutboundReturnByPurchase(PurchaseForReturnDto request)
        {
            _outboundAppService.CancelOutboundReturnByPurchase(request);
        }

        /// <summary>
        /// 异步回写出库单退货数量
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("AddOutboundReturnQtyByPurchase")]
        public OutboundReturnDto AddOutboundReturnQtyByPurchase(OutboundReturnDto request)
        {
            return _outboundAppService.AddOutboundReturnQtyByPurchase(request);
        }
    }
}
