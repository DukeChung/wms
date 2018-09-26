using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    /// <summary>
    /// RF盘点接口
    /// </summary>
    public interface IRFStockTakeAppService : IApplicationService
    {
        /// <summary>
        /// 初盘保存盘点单和盘点明细或修改盘点明细
        /// </summary>
        /// <param name="stockTakeFirst"></param>
        /// <returns></returns>
        RFCommResult SaveStockTake(StockTakeFirstDto stockTakeFirst);

        /// <summary>
        /// 初盘清单
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        RFStockTakeListingDto GetStockTakeFirstList(RFStockTakeQuery stockTakeQuery);

        /// <summary>
        /// 获取待初盘单据列表
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        Pages<RFStockTakeListDto> GetStockTakeFirstListByPaging(RFStockTakeQuery stockTakeQuery);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="upc"></param>
        /// <returns></returns>
        RFCheckStockTakeFirstDetailSkuDto CheckStockTakeFirstDetailSku(string upc);

        /// <summary>
        /// 获取初盘明细
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        List<StockTakeFirstListDto> GetStockTakeFirstDetailList(RFStockTakeQuery stockTakeQuery);

        /// <summary>
        /// 初盘扫描
        /// </summary>
        /// <param name="stockTakeFirstDto"></param>
        /// <returns></returns>
        RFCommResult StockTakeFirstScanning(StockTakeFirstDto stockTakeFirstDto);

        /// <summary>
        /// 复盘单据列表
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        Pages<RFStockTakeListDto> GetStockTakeSecondListByPage(RFStockTakeQuery stockTakeQuery);

        /// <summary>
        /// 复盘清单
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        List<StockTakeSecondListDto> GetStockTakeSecondList(RFStockTakeQuery stockTakeQuery);

        /// <summary>
        /// 复盘
        /// </summary>
        /// <returns></returns>
        RFCommResult StockTakeSecond(StockTakeSecondDto stockTakeSecond);

        /// <summary>
        /// 查询未盘点库存
        /// </summary>
        /// <param name="inventoryQuery"></param>
        /// <returns></returns>
        List<RFInventoryListDto> GetInventoryNoStockTakeList(RFInventoryQuery inventoryQuery);

        /// <summary>
        /// 修改盘点单状态
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        RFCommResult UpdateStockTakeStatus(RFStockTakeQuery stockTakeQuery);

        /// <summary>
        /// 查询盘点单
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        RFStockTakeListDto GetStockTakeByOrder(RFStockTakeQuery stockTakeQuery);
    }
}
