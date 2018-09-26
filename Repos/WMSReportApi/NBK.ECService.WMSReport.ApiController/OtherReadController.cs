using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Abp.WebApi.Controllers;
using NBK.ECService.WMSReport.Application.Interface;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Other;
using NBK.ECService.WMSReport.DTO.Query;

namespace NBK.ECService.WMSReport.ApiController
{
    [RoutePrefix("api/OtherRead")]
    public class OtherReadController : AbpApiController
    {
        private IOtherReadAppService _otherReadAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherReadAppService"></param>
        public OtherReadController(IOtherReadAppService otherReadAppService)
        {
            _otherReadAppService = otherReadAppService;
        }


        #region 出库
        /// <summary>
        /// 出库单管理
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundByPage")]
        public Pages<OutboundListDto> GetOutboundByPage(OutboundQuery request)
        {
            return _otherReadAppService.GetOutboundByPage(request);
        }


        /// <summary>
        /// 获取出库单明细
        /// </summary>
        /// <param name="sysid"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetOutboundBySysId")]
        public OutboundViewDto GetOutboundBySysId(Guid sysid, Guid warehouseSysId)
        {
            return _otherReadAppService.GetOutboundBySysId(sysid, warehouseSysId);
        }

        /// <summary>
        /// 获取出库单明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundDetailList")]
        public Pages<OutboundExceptionDto> GetOutboundDetailList(OutboundExceptionQueryDto request)
        {
            return _otherReadAppService.GetOutboundDetailList(request);
        }

        /// <summary>
        /// 获取出库单预包装差异
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetOutboundPrePackDiff")]
        public OutboundPrePackDiffDto GetOutboundPrePackDiff(Guid outboundSysId, Guid warehouseSysId)
        {
            return _otherReadAppService.GetOutboundPrePackDiff(outboundSysId, warehouseSysId);
        }

        /// <summary>
        /// 获取出库单散货箱差异
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetOutboundPreBulkPackDiff")]
        public OutboundPrePackDiffDto GetOutboundPreBulkPackDiff(Guid outboundSysId, Guid wareHouseSysId)
        {
            return _otherReadAppService.GetOutboundPreBulkPackDiff(outboundSysId, wareHouseSysId);
        }

        /// <summary>
        /// 获取出库单整件或者散件装箱数据
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetOutboundBox")]
        public OutboundBoxListDto GetOutboundBox(Guid outboundSysId, Guid warehouseSysId)
        {
            return _otherReadAppService.GetOutboundBox(outboundSysId, warehouseSysId);
        }


        /// <summary>
        /// 根据出库单ID获取异常明细
        /// </summary>
        /// <param name="sysid"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetOutbooundExceptionData")]
        public List<OutboundExceptionDtoList> GetOutbooundExceptionData(Guid sysid, Guid warehouseSysId)
        {
            return _otherReadAppService.GetOutbooundExceptionData(sysid, warehouseSysId);
        }

        #endregion

        #region 分拣
        /// <summary>
        /// 获取分页信息
        /// </summary>
        /// <param name="pickDetailQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPickDetailListDto")]
        public Pages<PickDetailListDto> GetPickDetailListDtoByPageInfo(PickDetailQuery pickDetailQuery)
        {
            return _otherReadAppService.GetPickDetailListDtoByPageInfo(pickDetailQuery);
        }

        /// <summary>
        /// 获取分页信息
        /// </summary>
        /// <param name="pickDetailQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPickOutboundListDto")]
        public Pages<PickOutboundListDto> GetPickOutboundListDtoByPageInfo(PickDetailQuery pickDetailQuery)
        {
            return _otherReadAppService.GetPickOutboundListDtoByPageInfo(pickDetailQuery);
        }
        #endregion

        #region 入库
        /// <summary>
        /// 获取分页信息
        /// </summary>
        /// <param name="purchaseQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPurchaseDtoList")]
        public Pages<PurchaseListDto> GetPurchaseDtoListByPageInfo(PurchaseQuery purchaseQuery)
        {
            return _otherReadAppService.GetPurchaseDtoListByPageInfo(purchaseQuery);
        }

        /// <summary>
        /// 退货入库
        /// </summary>
        /// <param name="purchaseQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPurchaseReturnDtoList")]
        public Pages<PurchaseReturnListDto> GetPurchaseReturnDtoListByPageInfo(PurchaseReturnQuery purchaseQuery)
        {
            return _otherReadAppService.GetPurchaseReturnDtoListByPageInfo(purchaseQuery);
        }

        /// <summary>
        /// 获取采购订单
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPurchaseViewDtoBySysId")]
        public PurchaseViewDto GetPurchaseViewDtoBySysId(Guid sysId, Guid warehouseSysId)
        {
            return _otherReadAppService.GetPurchaseViewDtoBySysId(sysId, warehouseSysId);
        }

        /// <summary>
        /// 获取退货入库单
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPurchaseReturnViewDtoBySysId")]
        public PurchaseReturnViewDto GetPurchaseReturnViewDtoBySysId(Guid sysId, Guid warehouseSysId)
        {
            return _otherReadAppService.GetPurchaseReturnViewDtoBySysId(sysId, warehouseSysId);
        }


        /// <summary>
        /// 获取收货单列表
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReceiptList")]
        public Pages<ReceiptListDto> GetReceiptList(ReceiptQuery receiptQuery)
        {
            return _otherReadAppService.GetReceiptList(receiptQuery);
        }

        /// <summary>
        /// 获取收货单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetReceiptViewById")]
        public ReceiptViewDto GetReceiptViewBySysId(Guid sysId, Guid warehouseSysId)
        {
            return _otherReadAppService.GetReceiptViewById(sysId, warehouseSysId);
        }

        /// <summary>
        /// 收货清单
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReceiptDetailViewList")]
        public List<ReceiptDetailViewDto> GetReceiptDetailViewList(Guid sysId, Guid warehouseSysId)
        {
            return _otherReadAppService.GetReceiptDetailViewList(sysId, warehouseSysId);
        }

        /// <summary>
        /// 收货批次清单
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReceiptDetailLotViewList")]
        public List<ReceiptDetailViewDto> GetReceiptDetailLotViewList(Guid sysId, Guid warehouseSysId)
        {
            return _otherReadAppService.GetReceiptDetailLotViewList(sysId, warehouseSysId);
        }

        /// <summary>
        /// 批次采集时获取收货清单明细
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReceiptDetailViewListByCollectionLot")]
        public List<ReceiptDetailViewDto> GetReceiptDetailViewListByCollectionLot(Guid receiptSysId, Guid warehouseSysId)
        {
            return _otherReadAppService.GetReceiptDetailViewListByCollectionLot(receiptSysId, warehouseSysId);
        }
        #endregion

        #region 库存转移
        /// <summary>
        /// 库存转移分页查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetStockTransferLotByPage")]
        public Pages<StockTransferLotListDto> GetStockTransferLotByPage(StockTransferQuery request)
        {
            return _otherReadAppService.GetStockTransferLotByPage(request);
        }
        #endregion
    }
}
