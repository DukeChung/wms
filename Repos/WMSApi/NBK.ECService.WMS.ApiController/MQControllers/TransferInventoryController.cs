using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NBK.ECService.WMS.DTO.MQ;

namespace NBK.ECService.WMS.ApiController.MQControllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/MQ/OrderManagement/TransferInventory")]
    [AccessLog]
    public class TransferInventoryController : AbpApiController
    {
        private ITransferInventoryAppService _transferInventoryAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transferInventoryAppService"></param>
        public TransferInventoryController(ITransferInventoryAppService transferInventoryAppService)
        {
            _transferInventoryAppService = transferInventoryAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void TransferInventoryAPI() { }

        /// <summary>
        /// 移仓单创建入库单
        /// </summary>
        /// <param name="transferInventoryDto"></param>
        /// <returns></returns>
        [HttpPost, Route("CreateTransferInventoryReceipt")]
        public CommonResponse CreateTransferInventoryReceipt(MQTransferInventoryDto transferInventoryDto)
        {
            return _transferInventoryAppService.CreateTransferInventoryReceipt(transferInventoryDto);
        }

        /// <summary>
        /// 移仓单创建出库单
        /// </summary>
        /// <param name="transferInventoryDto"></param>
        /// <returns></returns>
        [HttpPost, Route("AddOutboundByTransferInventory")]
        public CommonResponse AddOutboundByTransferInventory(MQTransferInventoryDto transferInventoryDto)
        {
            return _transferInventoryAppService.AddOutboundByTransferInventory(transferInventoryDto);
        }

        /// <summary>
        /// 移仓单修改状态
        /// </summary>
        /// <param name="transferInventoryDto"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateTransferInventoryStatus")]
        public CommonResponse UpdateTransferInventoryStatus(MQTransferInventoryDto transferInventoryDto)
        {
            return _transferInventoryAppService.UpdateTransferInventoryStatus(transferInventoryDto);
        }

    }
}
