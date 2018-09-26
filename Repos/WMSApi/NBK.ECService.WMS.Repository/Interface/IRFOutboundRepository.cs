using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IRFOutboundRepository : ICrudRepository
    {
        RFWaitingReviewDto GetWaitingReviewList(RFOutboundQuery outboundQuery);

        /// <summary>
        ///  获取预包装明细
        /// </summary>
        /// <param name="prePackQuery"></param>
        /// <returns></returns>
        List<RFPrePackDetailList> GetPrePackDetailList(RFPrePackQuery prePackQuery);

        List<RFPreBulkPackDetailDto> GetPreBulkPackDetailsByStorageCase(string storageCase, Guid warehouseSysId);

        List<RFPreBulkPackDetailDto> GetSkuByUPC(string upc);

        /// <summary>
        /// 根据UPC获取商品和包装
        /// </summary>
        /// <param name="upc"></param>
        /// <returns></returns>
        List<SkuPackDto> GetSkuPackListByUPC(string upc);

        void InsertOrUpdatePreBulkPackDetail(prebulkpack preBulkPack, RFPreBulkPackDetailDto preBulkPackDetailDto, sku sku, uom uom);

        /// <summary>
        /// 查询出库明细
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        List<RFOutboundDetailDto> GetOutboundDetailList(string outboundOrder, Guid warehouseSysId);

        /// <summary>
        /// 根据出库单ID和商品ID获取散货封箱已装箱数量
        /// </summary>
        /// <param name="outboundSysId">出库单ID</param>
        /// <param name="skuSysId">商品ID</param>
        /// <returns></returns>
        int GetPreBulkOutboundQty(Guid outboundSysId, Guid skuSysId);

        /// <summary>
        /// 获取交接箱复核差异
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<RFOutboundTransferOrderReviewDiffDto> GetTransferOrderReviewDiffList(RFOutboundTransferOrderQuery query);

        /// <summary>
        /// 获取待复核的出库单
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        Pages<RFOutboundReviewListDto> GetWaitingOutboundReviewListByPaging(RFOutboundQuery outboundQuery);

        /// <summary>
        /// 获取散货待复核明细
        /// </summary>
        /// <param name="outboundQuery"></param>
        /// <returns></returns>
        List<RFOutboundReviewDetailDto> GetWaitingSingleReviewDetails(RFOutboundQuery outboundQuery);
    }
}
