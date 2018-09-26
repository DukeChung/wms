using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IRFOutboundAppService : IApplicationService
    {
        RFWaitingReviewDto GetWaitingReviewList(RFOutboundQuery outboundQuery);

        RFCheckReviewResultDto CheckReviewDetailSku(RFCheckReviewDetailSkuDto checkReviewDetailSkuDto);

        RFReviewResultDto GetReviewFinishResult(RFReviewFinishDto reviewFinishDto);

        #region 整件预包装
        /// <summary>
        /// 获取预包装信息
        /// </summary>
        /// <param name="prePackQuery"></param>
        /// <returns></returns>
        RFPrePackDto GetPrePackByQuery(RFPrePackQuery prePackQuery);

        /// <summary>
        /// 验证商品是否存在
        /// </summary>
        /// <param name="prePackQuery"></param>
        /// <returns></returns>
        RFCommResult CheckPrePackDetailSku(RFPrePackQuery prePackQuery);

        /// <summary>
        /// 扫描预包装
        /// </summary>
        /// <param name="rfPrePackDto"></param>
        /// <returns></returns>
        RFCommResult ScanPrePack(RFPrePackDto rfPrePackDto);

        /// <summary>
        /// 查询预包装明细
        /// </summary>
        /// <param name="prePackQuery"></param>
        /// <returns></returns>
        List<RFPrePackDetailList> GetPrePackDetailList(RFPrePackQuery prePackQuery);
        #endregion

        RFPreBulkPackDto GetPreBulkPackDetailsByStorageCase(string storageCase, Guid warehouseSysId);

        List<RFPreBulkPackDetailDto> GetStorageCaseSkuList(string storageCase, Guid warehouseSysId);

        RFCheckPreBulkPackDetailSkuDto CheckPreBulkPackDetailSku(string upc, Guid warehouseSysId);

        RFCommResult GeneratePreBulkPackDetail(RFPreBulkPackDetailDto preBulkPackDetailDto);

        /// <summary>
        /// 查询出库明细
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        List<RFOutboundDetailDto> GetOutboundDetailList(string outboundOrder, Guid warehouseSysId);

        /// <summary>
        /// 根据UPC获取商品和包装
        /// </summary>
        /// <param name="upc"></param>
        /// <returns></returns>

        List<SkuPackDto> GetSkuPackListByUPC(string upc, Guid warehouseSysId);

        /// <summary>
        /// 根据容器号交接单号检查是否是同一出库单
        /// </summary>
        /// <param name="transferOrder"></param>
        /// <param name="storageCase"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        RFCommResult CheckTransferOrderAndPreBulkPack(string transferOrder, string storageCase, Guid warehouseSysId);

        /// <summary>
        /// 整件复核检查交接单是否有效
        /// </summary>
        /// <param name="transferOrder"></param>
        /// <param name="outboundOrder"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        RFCommResult CheckTransferOrder(string transferOrder, string outboundOrder, Guid warehouseSysId);

        /// <summary>
        /// 检查容器是否与出库单匹配
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <param name="storageCase"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        RFCommResult CheckStorageCaseIsAvailable(string outboundOrder, string storageCase, Guid warehouseSysId);

        /// <summary>
        /// 插入交接单数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        RFCommResult AddOutboundTransferOrder(RFOutboundTransferOrderQuery query);


        /// <summary>
        /// 交接箱复核完成封箱
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        RFCommResult SealedOutboundTransferBox(RFOutboundTransferOrderQuery query);

        /// <summary>
        /// 获取交接箱复核差异
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<RFOutboundTransferOrderReviewDiffDto> GetTransferOrderReviewDiffList(RFOutboundQuery query);

        /// <summary>
        /// 获取待复核的出库单
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        Pages<RFOutboundReviewListDto> GetWaitingOutboundReviewListByPaging(RFOutboundQuery outboundQuery);

        /// <summary>
        /// 判断扫描的单号是否待复核
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        RFCommResult CheckOutboundReviewOrder(RFOutboundQuery outboundQuery);

        /// <summary>
        /// 获取散货待复核明细
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        List<RFOutboundReviewDetailDto> GetWaitingSingleReviewDetails(RFOutboundQuery outboundQuery);

        /// <summary>
        /// 散件复核扫描
        /// </summary>
        /// <param name="scanningDto"></param>
        /// <returns></returns>
        RFCommResult SingleReviewScanning(RFSingleReviewScanningDto scanningDto);

        /// <summary>
        /// 散货复核完成
        /// </summary>
        /// <param name="finishDto"></param>
        /// <returns></returns>
        RFCommResult SingleReviewFinish(RFSingleReviewFinishDto finishDto);

        /// <summary>
        /// 整件复核扫描
        /// </summary>
        /// <param name="scanningDto"></param>
        /// <returns></returns>
        RFCommResult WholeReviewScanning(RFWholeReviewScanningDto scanningDto);

        /// <summary>
        /// 获取整件待复核明细
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        List<RFOutboundReviewDetailDto> GetWaitingWholeReviewDetails(RFOutboundQuery outboundQuery);

        /// <summary>
        /// 整件复核完成
        /// </summary>
        /// <param name="finishDto"></param>
        /// <returns></returns>
        RFCommResult WholeReviewFinish(RFWholeReviewFinishDto finishDto);

        /// <summary>
        /// 整单差异
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        RFOutboundReviewDiffDto GetOutboundReviewDiff(RFOutboundQuery outboundQuery);
    }
}
