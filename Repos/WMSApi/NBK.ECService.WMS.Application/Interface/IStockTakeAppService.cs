using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IStockTakeAppService : IApplicationService
    {
        /// <summary>
        /// 获取盘点列表
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        Pages<StockTakeListDto> GetStockTakeList(StockTakeQuery stockTakeQuery);

        /// <summary>
        /// 创建盘点单
        /// </summary>
        /// <param name="stockTakeDto"></param>
        /// <returns></returns>
        Guid AddStockTake(StockTakeDto stockTakeDto);

        /// <summary>
        /// 获取仓库下拉框
        /// </summary>
        /// <returns></returns>
        List<SelectItem> GetSelectWarehouse();

        /// <summary>
        /// 获取盘点单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        StockTakeViewDto GetStockTakeViewById(Guid sysId, Guid warehouseSysId);

        /// <summary>
        /// 获取盘点单明细
        /// </summary>
        /// <param name="stockTakeViewQuery"></param>
        /// <returns></returns>
        Pages<StockTakeDetailViewDto> GetStockTakeDetailList(StockTakeViewQuery stockTakeViewQuery);

        /// <summary>
        /// 获取盘点单差异
        /// </summary>
        /// <param name="stockTakeViewQuery"></param>
        /// <returns></returns>
        Pages<StockTakeDetailViewDto> GetStockTakeDiffList(StockTakeViewQuery stockTakeViewQuery);

        /// <summary>
        /// 盘点单明细复盘
        /// </summary>
        /// <param name="replayStockTakeDto"></param>
        void ReplayStockTakeDetail(ReplayStockTakeDto replayStockTakeDto);

        /// <summary>
        /// 获取生成损益单数据
        /// </summary>
        /// <param name="createAdjustmentDto"></param>
        /// <returns></returns>
        AdjustmentDto GetAdjustmentDto(CreateAdjustmentDto createAdjustmentDto);

        /// <summary>
        /// 获取商品分类下拉框
        /// </summary>
        /// <param name="parentSysId"></param>
        /// <returns></returns>
        List<SelectItem> GetSelectSkuClass(SelectSkuClassDto selectSkuClassDto);

        /// <summary>
        /// 盘点完成
        /// </summary>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        bool StockTakeComplete(StockTakeCompleteDto stockTake);

        /// <summary>
        /// 删除盘点单
        /// </summary>
        /// <param name="sysIds"></param>
        void DeleteStockTake(List<Guid> sysIds, Guid warehouseSysId);

        /// <summary>
        /// 盘点汇总报告
        /// </summary>
        /// <param name="stockTakeReportQuery"></param>
        /// <returns></returns>
        Pages<StockTakeReportListDto> GetStockTakeReport(StockTakeReportQuery stockTakeReportQuery);

        /// <summary>
        /// 获取待盘点商品信息
        /// </summary>
        /// <param name="stockTakeSkuQuery"></param>
        /// <returns></returns>
        Pages<StockTakeSkuListDto> GetWaitingStockTakeSkuList(StockTakeSkuQuery stockTakeSkuQuery);

        /// <summary>
        /// 创建盘点单
        /// </summary>
        /// <param name="newStockTakeDto"></param>
        void NewStockTake(NewStockTakeDto newStockTakeDto);

        /// <summary>
        /// 开始盘点
        /// </summary>
        /// <param name="stockTakeStartDto"></param>
        void StockTakeStart(StockTakeStartDto stockTakeStartDto);
    }
}
