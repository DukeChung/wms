using Abp.WebApi.Controllers;
using FortuneLab.WebApiClient;
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
    /// 订单管理_交接管理 
    /// </summary>
    [RoutePrefix("api/Order/OutboundTransferOrder")]
    [AccessLog]
    public class OutboundTransferOrderController : AbpApiController
    {

        private IOutboundTransferOrderAppService _outboundTransferOrderAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outboundTransferOrderAppService"></param>
        public OutboundTransferOrderController(IOutboundTransferOrderAppService outboundTransferOrderAppService)
        {
            _outboundTransferOrderAppService = outboundTransferOrderAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void OutboundTransferOrderAPI() { }


        /// <summary>
        /// 分页获取预包装单数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundTransferOrderByPage")]
        public Pages<OutboundTransferOrderDto> GetOutboundTransferOrderByPage(OutboundTransferOrderQuery request)
        {
            return _outboundTransferOrderAppService.GetOutboundTransferOrderByPage(request);
        }


        /// <summary>
        /// 获取交接明细数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetDataBySysId")]
        public OutboundTransferOrderDto GetDataBySysId(Guid sysId, Guid warehouseSysId)
        {
            return _outboundTransferOrderAppService.GetDataBySysId(sysId, warehouseSysId);
        }

        /// <summary>
        /// 分页获取预包装单数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundTransferBox")]
        public List<OutboundTransferPrintDto> GetOutboundTransferBox(List<Guid> request, Guid warehouseSysId)
        {
            return _outboundTransferOrderAppService.GetOutboundTransferBox(request, warehouseSysId);
        }

        /// <summary>
        /// 根据出库单获取所有交接单
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOutboundTransferOrder")]
        public List<OutboundTransferPrintDto> GetOutboundTransferOrder(OutboundTransferOrderQuery dto)
        {
            return _outboundTransferOrderAppService.GetOutboundTransferOrder(dto);
        }


        /// <summary>
        /// 更新交接明细内容
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateTransferOrderSku")]
        public CommonResponse UpdateTransferOrderSku(OutboundTransferOrderMoveDto dto)
        {
            return _outboundTransferOrderAppService.UpdateTransferOrderSku(dto);
        }
    }
}
