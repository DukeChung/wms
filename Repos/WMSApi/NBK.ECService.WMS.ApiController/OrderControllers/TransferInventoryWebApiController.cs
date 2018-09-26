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

namespace NBK.ECService.WMS.ApiController.OrderControllers
{
    /// <summary>
    /// 订单管理_移仓单管理
    /// </summary>
    [RoutePrefix("api/Order/TransferInventoryWebApi")]
    [AccessLog]
    public class TransferInventoryWebApiController : AbpApiController
    {
        private ITransferInventoryAppService _transferInventoryAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transferInventoryAppService"></param>
        public TransferInventoryWebApiController(ITransferInventoryAppService transferInventoryAppService)
        {
            _transferInventoryAppService = transferInventoryAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void TransferInventoryWebApiAPI() { }


        /// <summary>
        /// 分页获取移仓单数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetTransferinventoryByPage")]
        public Pages<TransferinventoryListDto> GetTransferinventoryByPage(TransferinventoryQuery request)
        {
            return _transferInventoryAppService.GetTransferinventoryByPage(request);
        }

        /// <summary>
        /// 获取移仓单数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpPost, Route("GetTransferinventoryBySysId")]
        public TransferInventoryViewDto GetTransferinventoryBySysId(Guid sysId, Guid warehouseSysId)
        {
            return _transferInventoryAppService.GetTransferinventoryBySysId(sysId, warehouseSysId);
        }

        /// <summary>
        /// 作废移仓单
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("ObsoleteTransferinventory")]
        public bool ObsoleteTransferinventory(TransferinventoryUpdateQuery dto)
        {
            return _transferInventoryAppService.ObsoleteTransferinventory(dto);
        }
    }
}
