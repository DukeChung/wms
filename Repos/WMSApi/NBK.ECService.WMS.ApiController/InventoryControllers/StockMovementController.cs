using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMS.ApiController.InventoryControllers
{
    /// <summary>
    /// 库存移动
    /// </summary>
    [RoutePrefix("api/Inventory/StockMovement")]
    [AccessLog]
    public class StockMovementController : AbpApiController
    {
        private IStockMovementAppService _stockMovementAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stockMovementAppService"></param>
        public StockMovementController(IStockMovementAppService stockMovementAppService)
        {
            _stockMovementAppService = stockMovementAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void StockMovementAPI() { }

        /// <summary>
        /// 获取库存移动SKU列表
        /// </summary>
        /// <param name="stockMovementSkuQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetStockMovementSkuList")]
        public Pages<StockMovementSkuDto> GetStockMovementSkuList(StockMovementSkuQuery stockMovementSkuQuery)
        {
            return _stockMovementAppService.GetStockMovementSkuList(stockMovementSkuQuery);
        }

        /// <summary>
        /// 获取库存移动信息
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="loc"></param>
        /// <param name="lot"></param>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetStockMovement")]
        public StockMovementDto GetStockMovement(Guid skuSysId, string loc, string lot, Guid wareHouseSysId)
        {
            return _stockMovementAppService.GetStockMovement(skuSysId, loc, lot, wareHouseSysId);
        }

        /// <summary>
        /// 保存调整
        /// </summary>
        /// <param name="stockMovementDto"></param>
        [HttpPost, Route("SaveStockMovement")]
        public void SaveStockMovement(StockMovementDto stockMovementDto)
        {
            _stockMovementAppService.SaveStockMovement(stockMovementDto);
        }

        /// <summary>
        /// 获取移动单列表
        /// </summary>
        /// <param name="stockMovementQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetStockMovementList")]
        public Pages<StockMovementDto> GetStockMovementList(StockMovementQuery stockMovementQuery)
        {
            return _stockMovementAppService.GetStockMovementList(stockMovementQuery);
        }

        /// <summary>
        /// 确认移动
        /// </summary>
        /// <param name="stockMovementOperationDto"></param>
        [HttpPost, Route("ConfirmStockMovement")]
        public void ConfirmStockMovement(StockMovementOperationDto stockMovementOperationDto)
        {
            try
            {
                _stockMovementAppService.ConfirmStockMovement(stockMovementOperationDto);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("确认变更失败，失败原因：数据重复并发提交");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取消移动
        /// </summary>
        /// <param name="stockMovementOperationDto"></param>
        [HttpPost, Route("CancelStockMovement")]
        public void CancelStockMovement(StockMovementOperationDto stockMovementOperationDto)
        {
            _stockMovementAppService.CancelStockMovement(stockMovementOperationDto);
        }

        /// <summary>
        /// 库位变更导入
        /// </summary>
        /// <param name="list"></param>
        [HttpPost, Route("ImportStockMovementList")]
        public void ImportStockMovementList(ImportStockMovement list)
        {
            _stockMovementAppService.ImportStockMovementList(list);
        }
    }
}
