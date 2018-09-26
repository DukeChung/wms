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

namespace NBK.ECService.WMS.ApiController.InventoryControllers
{
    /// <summary>
    /// 盘点
    /// </summary>
    [RoutePrefix("api/Inventory/StockTake")]
    [AccessLog]
    public class StockTakeController : AbpApiController
    {
        private IStockTakeAppService _stockTakeAppService;

        /// <summary>
        /// 
        /// </summary>
        public StockTakeController(IStockTakeAppService stockTakeAppService)
        {
            _stockTakeAppService = stockTakeAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void StockTakeAPI() { }

        /// <summary>
        /// 获取盘点列表
        /// </summary>
        /// <param name="stockTakeQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetStockTakeList")]
        public Pages<StockTakeListDto> GetStockTakeList(StockTakeQuery stockTakeQuery)
        {
            return _stockTakeAppService.GetStockTakeList(stockTakeQuery);
        }

        /// <summary>
        /// 创建盘点单
        /// </summary>
        /// <param name="stockTakeDto"></param>
        /// <returns></returns>
        [HttpPost, Route("AddStockTake")]
        public Guid AddStockTake(StockTakeDto stockTakeDto)
        {
            return _stockTakeAppService.AddStockTake(stockTakeDto);
        }

        /// <summary>
        /// 获取仓库下拉框
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetSelectWarehouse")]
        public List<SelectItem> GetSelectWarehouse()
        {
            return _stockTakeAppService.GetSelectWarehouse();
        }

        /// <summary>
        /// 获取盘点单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetStockTakeViewById")]
        public StockTakeViewDto GetStockTakeViewById(Guid sysId, Guid warehouseSysId)
        {
            return _stockTakeAppService.GetStockTakeViewById(sysId, warehouseSysId);
        }

        /// <summary>
        /// 获取盘点单明细
        /// </summary>
        /// <param name="stockTakeViewQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetStockTakeDetailList")]
        public Pages<StockTakeDetailViewDto> GetStockTakeDetailList(StockTakeViewQuery stockTakeViewQuery)
        {
            return _stockTakeAppService.GetStockTakeDetailList(stockTakeViewQuery);
        }

        /// <summary>
        /// 获取盘点单差异
        /// </summary>
        /// <param name="stockTakeViewQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetStockTakeDiffList")]
        public Pages<StockTakeDetailViewDto> GetStockTakeDiffList(StockTakeViewQuery stockTakeViewQuery)
        {
            return _stockTakeAppService.GetStockTakeDiffList(stockTakeViewQuery);
        }

        /// <summary>
        /// 盘点单明细复盘
        /// </summary>
        /// <param name="replayStockTakeDto"></param>
        [HttpPost, Route("ReplayStockTakeDetail")]
        public void ReplayStockTakeDetail(ReplayStockTakeDto replayStockTakeDto)
        {
            _stockTakeAppService.ReplayStockTakeDetail(replayStockTakeDto);
        }

        /// <summary>
        /// 获取生成损益单数据
        /// </summary>
        /// <param name="createAdjustmentDto"></param>
        /// <returns></returns>
        [HttpPost, Route("GetAdjustmentDto")]
        public AdjustmentDto GetAdjustmentDto(CreateAdjustmentDto createAdjustmentDto)
        {
            return _stockTakeAppService.GetAdjustmentDto(createAdjustmentDto);
        }

        /// <summary>
        /// 盘点完成
        /// </summary>
        /// <param name="stockTakeCompleteDto"></param>
        [HttpPost, Route("StockTakeComplete")]
        public bool StockTakeComplete(StockTakeCompleteDto stockTakeCompleteDto)
        {
            return _stockTakeAppService.StockTakeComplete(stockTakeCompleteDto);
        }

        /// <summary>
        /// 删除盘点单
        /// </summary>
        /// <param name="sysIds"></param>
        [HttpPost, Route("DeleteStockTake")]
        public void DeleteStockTake(List<Guid> sysIds,Guid warehouseSysId)
        {
            _stockTakeAppService.DeleteStockTake(sysIds, warehouseSysId);
        }

        /// <summary>
        /// 盘点汇总报告
        /// </summary>
        /// <param name="stockTakeReportQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetStockTakeReport")]
        public Pages<StockTakeReportListDto> GetStockTakeReport(StockTakeReportQuery stockTakeReportQuery)
        {
            return _stockTakeAppService.GetStockTakeReport(stockTakeReportQuery);
        }

        /// <summary>
        /// 获取商品分类下拉框
        /// </summary>
        /// <param name="selectSkuClassDto"></param>
        /// <returns></returns>
        [HttpPost, Route("GetSelectSkuClass")]
        public List<SelectItem> GetSelectSkuClass(SelectSkuClassDto selectSkuClassDto)
        {
            return _stockTakeAppService.GetSelectSkuClass(selectSkuClassDto);
        }

        /// <summary>
        /// 获取待盘点商品信息
        /// </summary>
        /// <param name="stockTakeSkuQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWaitingStockTakeSkuList")]
        public Pages<StockTakeSkuListDto> GetWaitingStockTakeSkuList(StockTakeSkuQuery stockTakeSkuQuery)
        {
            return _stockTakeAppService.GetWaitingStockTakeSkuList(stockTakeSkuQuery);
        }

        /// <summary>
        /// 创建盘点单
        /// </summary>
        /// <param name="newStockTakeDto"></param>
        [HttpPost, Route("NewStockTake")]
        public void NewStockTake(NewStockTakeDto newStockTakeDto)
        {
            _stockTakeAppService.NewStockTake(newStockTakeDto);
        }

        /// <summary>
        /// 开始盘点
        /// </summary>
        /// <param name="stockTakeStartDto"></param>
        [HttpPost, Route("StockTakeStart")]
        public void StockTakeStart(StockTakeStartDto stockTakeStartDto)
        {
            _stockTakeAppService.StockTakeStart(stockTakeStartDto);
        }
    }
}
