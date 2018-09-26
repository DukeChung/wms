using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.MQ;
using NBK.ECService.WMS.DTO.Outbound;
using NBK.ECService.WMS.DTO.ThirdParty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMS.ApiController.MQControllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/MQ/OrderManagement/MQOutbound")]
    [AccessLog]
    public class MQOutboundController : AbpApiController
    {
        private IOutboundAppService _outboundAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outboundAppService"></param>
        public MQOutboundController(IOutboundAppService outboundAppService)
        {
            _outboundAppService = outboundAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void MQOutboundAPI() { }

        /// <summary>
        /// 快速发货(消息队列调用)
        /// </summary>
        /// <param name="outboundQuickDeliveryDto"></param>
        /// <returns></returns>
        [HttpPost, Route("OutboundQuickDelivery")]
        public CommonResponse OutboundQuickDelivery(OutboundQuickDeliveryDto outboundQuickDeliveryDto)
        {
            return _outboundAppService.OutboundQuickDelivery(outboundQuickDeliveryDto);
        }

        /// <summary>
        /// 异步创建出库单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("CreateOutboundByMQ")]
        public CommonResponse CreateOutboundByMQ(MQProcessDto<ThirdPartyOutboundDto> request)
        {
            return _outboundAppService.CreateOutboundByMQ(request);
        }
    }
}
