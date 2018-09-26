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
    /// <summary>
    /// RF库存
    /// </summary>
    [RoutePrefix("api/RFInventory")]
    [AccessLog]
    public class RFInventoryController : AbpApiController
    {
        IRFInventoryAppService _rfInventoryAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rfInventoryAppService"></param>
        public RFInventoryController(IRFInventoryAppService rfInventoryAppService)
        {
            _rfInventoryAppService = rfInventoryAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void RFInventoryAPI() { }

        /// <summary>
        /// RF库存查询
        /// </summary>
        /// <param name="invSkuLocQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetInvSkuLocList")]
        public List<RFInvSkuLocListDto> GetInvSkuLocList(RFInvSkuLocQuery invSkuLocQuery)
        {
            return _rfInventoryAppService.GetInvSkuLocList(invSkuLocQuery);
        }

        #region RF库位变更
        /// <summary>
        /// RF库位变更
        /// </summary>
        /// <param name="rfStockMovementDto"></param>
        /// <returns></returns>
        [HttpPost, Route("StockMovement")]
        public RFCommResult StockMovement(RFStockMovementDto rfStockMovementDto)
        {
            return _rfInventoryAppService.StockMovement(rfStockMovementDto);
        }
        #endregion
    }
}
