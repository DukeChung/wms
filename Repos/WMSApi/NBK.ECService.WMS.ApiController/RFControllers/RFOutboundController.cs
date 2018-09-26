using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMS.ApiController.RFControllers
{
    [RoutePrefix("api/RFOutbound")]
    [AccessLog]
    public class RFOutboundController : AbpApiController
    {
        IOutboundAppService _outboundAppService;
        IRFOutboundAppService _rfOutboundAppService;

        /// <summary>
        /// 系统构造
        /// </summary>
        public RFOutboundController(IOutboundAppService outboundAppService, IRFOutboundAppService rfOutboundAppService)
        {
            _outboundAppService = outboundAppService;
            _rfOutboundAppService = rfOutboundAppService;
        }
        [HttpGet]
        public void RFOutboundAPI()
        {
        }

        /// <summary>
        /// RF获取发货信息
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
        /// RF发货
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("SaveDeliveryByVanningSysId")]
        public RFCommResult SaveDeliveryByVanningSysId(RFScanDeliveryDto request)
        {
            var result = new RFCommResult() { IsSucess = false };
            try
            {
                _outboundAppService.SaveDeliveryByVanningSysId(request.vanningSysIds, request.currentUserName, request.currentUserId, request.wareHouseSysId);
                result.IsSucess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 判断出库单是否存在
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("CheckOutboundExists")]
        public RFCommResult CheckOutboundExists(OutboundQuery request)
        {
            var result = new RFCommResult();
            if (_outboundAppService.GetOutboundOrderByOrderId(request) != null)
            {
                result.IsSucess = true;
            }
            else
            {
                result.IsSucess = false;
                result.Message = "出库单不存在";
            }
            return result;
        }

        /// <summary>
        /// 待复核出库单明细
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWaitingReviewList")]
        public RFWaitingReviewDto GetWaitingReviewList(RFOutboundQuery outboundQuery)
        {
            return _rfOutboundAppService.GetWaitingReviewList(outboundQuery);
        }

        /// <summary>
        /// 检查待复核商品信息
        /// </summary>
        /// <param name="checkReviewDetailSkuDto"></param>
        /// <returns></returns>
        [HttpPost, Route("CheckReviewDetailSku")]
        public RFCheckReviewResultDto CheckReviewDetailSku(RFCheckReviewDetailSkuDto checkReviewDetailSkuDto)
        {
            return _rfOutboundAppService.CheckReviewDetailSku(checkReviewDetailSkuDto);
        }

        /// <summary>
        /// 复核完成
        /// </summary>
        /// <param name="reviewFinishDto"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReviewFinishResult")]
        public RFReviewResultDto GetReviewFinishResult(RFReviewFinishDto reviewFinishDto)
        {
            return _rfOutboundAppService.GetReviewFinishResult(reviewFinishDto);
        }

        #region 整件预包装
        /// <summary>
        /// 获取预包装信息
        /// </summary>
        /// <param name="prePackQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPrePackByQuery")]
        public RFPrePackDto GetPrePackByQuery(RFPrePackQuery prePackQuery)
        {
            return _rfOutboundAppService.GetPrePackByQuery(prePackQuery);
        }

        /// <summary>
        /// 验证商品是否存在
        /// </summary>
        /// <param name="prePackQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("CheckPrePackDetailSku")]
        public RFCommResult CheckPrePackDetailSku(RFPrePackQuery prePackQuery)
        {
            return _rfOutboundAppService.CheckPrePackDetailSku(prePackQuery);
        }

        /// <summary>
        /// 扫描预包装
        /// </summary>
        /// <param name="rfPrePackDto"></param>
        /// <returns></returns>
        [HttpPost, Route("ScanPrePack")]
        public RFCommResult ScanPrePack(RFPrePackDto rfPrePackDto)
        {
            return _rfOutboundAppService.ScanPrePack(rfPrePackDto);
        }

        /// <summary>
        /// 分页查询预包装明细
        /// </summary>
        /// <param name="prePackQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPrePackDetailList")]
        public List<RFPrePackDetailList> GetPrePackDetailList(RFPrePackQuery prePackQuery)
        {
            return _rfOutboundAppService.GetPrePackDetailList(prePackQuery);
        }
        #endregion

        /// <summary>
        /// 根据箱号获取散货预包装明细
        /// </summary>
        /// <param name="storageCase"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPreBulkPackDetailsByStorageCase")]
        public RFPreBulkPackDto GetPreBulkPackDetailsByStorageCase(string storageCase, Guid warehouseSysId)
        {
            return _rfOutboundAppService.GetPreBulkPackDetailsByStorageCase(storageCase, warehouseSysId);
        }


        /// <summary>
        /// 根据容器获取商品list
        /// </summary>
        /// <param name="storageCase"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetStorageCaseSkuList")]
        public List<RFPreBulkPackDetailDto> GetStorageCaseSkuList(string storageCase, Guid warehouseSysId)
        {
            return _rfOutboundAppService.GetStorageCaseSkuList(storageCase, warehouseSysId);
        }


        /// <summary>
        /// 检查散货商品是否存在
        /// </summary>
        /// <param name="upc"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("CheckPreBulkPackDetailSku")]
        public RFCheckPreBulkPackDetailSkuDto CheckPreBulkPackDetailSku(string upc, Guid warehouseSysId)
        {
            return _rfOutboundAppService.CheckPreBulkPackDetailSku(upc, warehouseSysId);
        }

        /// <summary>
        /// 生成散货预包装明细
        /// </summary>
        /// <param name="preBulkPackDetailDto"></param>
        /// <returns></returns>
        [HttpPost, Route("GeneratePreBulkPackDetail")]
        public RFCommResult GeneratePreBulkPackDetail(RFPreBulkPackDetailDto preBulkPackDetailDto)
        {
            return _rfOutboundAppService.GeneratePreBulkPackDetail(preBulkPackDetailDto);
        }

        /// <summary>
        /// 查询出库明细
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetOutboundDetailList")]
        public List<RFOutboundDetailDto> GetOutboundDetailList(string outboundOrder, Guid warehouseSysId)
        {
            return _rfOutboundAppService.GetOutboundDetailList(outboundOrder, warehouseSysId);
        }


        /// <summary>
        /// 根据容器号交接单号检查是否是同一出库单
        /// </summary>
        /// <param name="transferOrder"></param>
        /// <param name="storageCase"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("CheckTransferOrderAndPreBulkPack")]
        public RFCommResult CheckTransferOrderAndPreBulkPack(string transferOrder, string storageCase, Guid warehouseSysId)
        {
            return _rfOutboundAppService.CheckTransferOrderAndPreBulkPack(transferOrder, storageCase, warehouseSysId);
        }

        /// <summary>
        /// 整件复核检查交接单是否有效
        /// </summary>
        /// <param name="transferOrder"></param>
        /// <param name="outboundOrder"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("CheckTransferOrder")]
        public RFCommResult CheckTransferOrder(string transferOrder, string outboundOrder, Guid warehouseSysId)
        {
            return _rfOutboundAppService.CheckTransferOrder(transferOrder, outboundOrder, warehouseSysId);
        }

        /// <summary>
        /// 检查容器是否与出库单匹配
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <param name="storageCase"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("CheckStorageCaseIsAvailable")]
        public RFCommResult CheckStorageCaseIsAvailable(string outboundOrder, string storageCase, Guid warehouseSysId)
        {
            return _rfOutboundAppService.CheckStorageCaseIsAvailable(outboundOrder, storageCase, warehouseSysId);
        }

        /// <summary>
        /// 向交接单中插入数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost, Route("AddOutboundTransferOrder")]
        public RFCommResult AddOutboundTransferOrder(RFOutboundTransferOrderQuery query)
        {
            return _rfOutboundAppService.AddOutboundTransferOrder(query);
        }

        /// <summary>
        /// 交接箱复核完成封箱
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost, Route("SealedOutboundTransferBox")]
        public RFCommResult SealedOutboundTransferBox(RFOutboundTransferOrderQuery query)
        {
            return _rfOutboundAppService.SealedOutboundTransferBox(query);
        }

        /// <summary>
        /// 交接箱复核完成封箱
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost, Route("GetTransferOrderReviewDiffList")]
        public List<RFOutboundTransferOrderReviewDiffDto> GetTransferOrderReviewDiffList(RFOutboundQuery query)
        {
            return _rfOutboundAppService.GetTransferOrderReviewDiffList(query);
        }

        /// <summary>
        /// 获取待复核的出库单
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWaitingOutboundReviewListByPaging")]
        public Pages<RFOutboundReviewListDto> GetWaitingOutboundReviewListByPaging(RFOutboundQuery outboundQuery)
        {
            return _rfOutboundAppService.GetWaitingOutboundReviewListByPaging(outboundQuery);
        }

        /// <summary>
        /// 判断扫描的单号是否待复核
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("CheckOutboundReviewOrder")]
        public RFCommResult CheckOutboundReviewOrder(RFOutboundQuery outboundQuery)
        {
            return _rfOutboundAppService.CheckOutboundReviewOrder(outboundQuery);
        }

        /// <summary>
        /// 获取散货待复核明细
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWaitingSingleReviewDetails")]
        public List<RFOutboundReviewDetailDto> GetWaitingSingleReviewDetails(RFOutboundQuery outboundQuery)
        {
            return _rfOutboundAppService.GetWaitingSingleReviewDetails(outboundQuery);
        }

        /// <summary>
        /// 散件复核扫描
        /// </summary>
        /// <param name="scanningDto"></param>
        /// <returns></returns>
        [HttpPost, Route("SingleReviewScanning")]
        public RFCommResult SingleReviewScanning(RFSingleReviewScanningDto scanningDto)
        {
            return _rfOutboundAppService.SingleReviewScanning(scanningDto);
        }

        /// <summary>
        /// 散货复核完成
        /// </summary>
        /// <param name="finishDto"></param>
        /// <returns></returns>
        [HttpPost, Route("SingleReviewFinish")]
        public RFCommResult SingleReviewFinish(RFSingleReviewFinishDto finishDto)
        {
            return _rfOutboundAppService.SingleReviewFinish(finishDto);
        }

        /// <summary>
        /// 整件复核扫描
        /// </summary>
        /// <param name="scanningDto"></param>
        /// <returns></returns>
        [HttpPost, Route("WholeReviewScanning")]
        public RFCommResult WholeReviewScanning(RFWholeReviewScanningDto scanningDto)
        {
            return _rfOutboundAppService.WholeReviewScanning(scanningDto);
        }

        /// <summary>
        /// 获取整件待复核明细
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWaitingWholeReviewDetails")]
        public List<RFOutboundReviewDetailDto> GetWaitingWholeReviewDetails(RFOutboundQuery outboundQuery)
        {
            return _rfOutboundAppService.GetWaitingWholeReviewDetails(outboundQuery);
        }

        /// <summary>
        /// 整件复核完成
        /// </summary>
        /// <param name="finishDto"></param>
        /// <returns></returns>
        [HttpPost, Route("WholeReviewFinish")]
        public RFCommResult WholeReviewFinish(RFWholeReviewFinishDto finishDto)
        {
            return _rfOutboundAppService.WholeReviewFinish(finishDto);
        }

        /// <summary>
        /// 整单差异
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundReviewDiff")]
        public RFOutboundReviewDiffDto GetOutboundReviewDiff(RFOutboundQuery outboundQuery)
        {
            return _rfOutboundAppService.GetOutboundReviewDiff(outboundQuery);
        }
    }
}
