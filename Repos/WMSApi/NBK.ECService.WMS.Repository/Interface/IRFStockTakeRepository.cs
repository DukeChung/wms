using System;
using System.Collections.Generic;
using NBK.ECService.WMS.DTO;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IRFStockTakeRepository
    {
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
        List<RFStockTakeFirstDetailDto> GetSkuByUPC(string upc);

        /// <summary>
        /// 获取初盘明细
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        List<StockTakeFirstListDto> GetStockTakeFirstDetailList(RFStockTakeQuery stockTakeQuery);

        /// <summary>
        /// 盘点单明细原材料和成品是否混合
        /// </summary>
        /// <param name="stockTakeSysId"></param>
        /// <param name="isMaterial"></param>
        /// <returns></returns>
        bool GetStockTakeFirstMaterialProduct(Guid stockTakeSysId, bool? isMaterial);

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
        /// 查询未盘点库存
        /// </summary>
        /// <param name="inventoryQuery"></param>
        /// <returns></returns>
        List<RFInventoryListDto> GetInventoryNoStockTakeList(RFInventoryQuery inventoryQuery);
    }
}
