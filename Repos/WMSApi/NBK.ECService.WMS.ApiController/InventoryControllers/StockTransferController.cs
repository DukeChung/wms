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
    /// 容器管理
    /// </summary>
    [RoutePrefix("api/Inventory/StockTransfer")]
    [AccessLog]
    public class StockTransferController : AbpApiController
    {
        IStockTransferAppService _stockTransferAppService;

        public StockTransferController(IStockTransferAppService stockTransferAppService)
        {
            _stockTransferAppService = stockTransferAppService;
        }

        [HttpGet]
        public void StockTransferAPI()
        {
        }

        /// <summary>
        /// 分页查询库存批次信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost, Route("GetStockTransferLotByPage")]
        public Pages<StockTransferLotListDto> GetStockTransferLotByPage(StockTransferQuery request)
        {
            return _stockTransferAppService.GetStockTransferLotByPage(request);
        }

        /// <summary>
        /// 获取单条批次库存信息
        /// </summary>
        /// <param name="sysid"></param>
        /// <returns></returns>
        [HttpGet, Route("GetStockTransferBySysId")]
        public StockTransferDto GetStockTransferBySysId(Guid sysid, Guid warehouseSysId)
        {
            return _stockTransferAppService.GetStockTransferBySysId(sysid, warehouseSysId);
        }

        /// <summary>
        /// 创建批次转移单
        /// </summary>
        /// <param name="st"></param>
        [HttpPost, Route("CreateStockTransfer")]
        public void CreateStockTransfer(StockTransferDto st)
        {
            _stockTransferAppService.CreateStockTransfer(st);
        }

        /// <summary>
        /// 分页获取批次转移单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetStockTransferOrderByPage")]
        public Pages<StockTransferDto> GetStockTransferOrderByPage(StockTransferQuery request)
        {
            return _stockTransferAppService.GetStockTransferOrderByPage(request);
        }

        /// <summary>
        /// 确认转移
        /// </summary>
        /// <param request=""></param>
        [HttpPost, Route("StockTransferOperation")]
        public void StockTransferOperation(StockTransferDto request)
        {
            try
            {
                _stockTransferAppService.StockTransferOperation(request);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("确认转移失败，失败原因：数据重复并发提交");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取消转移
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("StockTransferCancel")]
        public void StockTransferCancel(StockTransferDto request)
        {
            _stockTransferAppService.StockTransferCancel(request);
        }

        /// <summary>
        /// 获取转移单明细内容
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetStockTransferOrderBySysId")]
        public StockTransferDto GetStockTransferOrderBySysId(Guid sysId, Guid warehouseSysId)
        {
            return _stockTransferAppService.GetStockTransferOrderBySysId(sysId, warehouseSysId);
        }
    }
}
