using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IStockTakeRepository : ICrudRepository
    {
        /// <summary>
        /// 获取盘点列表
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        Pages<StockTakeListDto> GetStockTakeListByPaging(StockTakeQuery stockTakeQuery);

        /// <summary>
        /// 获取盘点单明细
        /// </summary>
        /// <param name="stockTakeViewQuery"></param>
        /// <returns></returns>
        Pages<StockTakeDetailViewDto> GetStockTakeDetailListByPaging(StockTakeViewQuery stockTakeViewQuery);

        /// <summary>
        /// 获取盘点单差异
        /// </summary>
        /// <param name="stockTakeViewQuery"></param>
        /// <returns></returns>
        Pages<StockTakeDetailViewDto> GetStockTakeDiffListByPaging(StockTakeViewQuery stockTakeViewQuery);

        /// <summary>
        /// 盘点汇总报告
        /// </summary>
        /// <param name="stockTakeReportQuery"></param>
        /// <returns></returns>
        Pages<StockTakeReportListDto> GetStockTakeReport(StockTakeReportQuery stockTakeReportQuery);

        /// <summary>
        /// 根据库位获取待盘点商品
        /// </summary>
        /// <param name="stockTakeSkuQuery"></param>
        /// <returns></returns>
        Pages<StockTakeSkuListDto> GetWaitingStockTakeSkuByLocation(StockTakeSkuQuery stockTakeSkuQuery);

        /// <summary>
        /// 根据商品信息获取待盘点商品
        /// </summary>
        /// <param name="stockTakeSkuQuery"></param>
        /// <returns></returns>
        Pages<StockTakeSkuListDto> GetWaitingStockTakeSkuBySkuInfo(StockTakeSkuQuery stockTakeSkuQuery);

        /// <summary>
        /// 根据交易信息获取待盘点商品
        /// </summary>
        /// <param name="stockTakeSkuQuery"></param>
        /// <returns></returns>
        Pages<StockTakeSkuListDto> GetWaitingStockTakeSkuByInvTrans(StockTakeSkuQuery stockTakeSkuQuery);

        /// <summary>
        /// 根据库位获取盘点明细
        /// </summary>
        /// <param name="newStockTakeDto"></param>
        /// <returns></returns>
        List<StockTakeSkuListDto> GeStockTakeDetailsByLocation(NewStockTakeDto newStockTakeDto);

        /// <summary>
        /// 根据商品获取盘点明细
        /// </summary>
        /// <param name="newStockTakeDto"></param>
        /// <returns></returns>
        List<StockTakeSkuListDto> GetStockTakeDetailsBySkuInfo(NewStockTakeDto newStockTakeDto);

        /// <summary>
        /// 根据交易获取盘点明细
        /// </summary>
        /// <param name="newStockTakeDto"></param>
        /// <returns></returns>
        List<StockTakeSkuListDto> GeStockTakeDetailsByInvTrans(NewStockTakeDto newStockTakeDto);
    }
}
