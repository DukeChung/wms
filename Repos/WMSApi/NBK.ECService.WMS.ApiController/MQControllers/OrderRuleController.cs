using System.Web.Http;
using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.MQ.OrderRule;

namespace NBK.ECService.WMS.ApiController.MQControllers
{
    [RoutePrefix("api/MQ/OrderManagement/OrderRule")]
    [AccessLog]
    public class OrderRuleController : AbpApiController
    { 
        private IOrderRuleSettingAppService _orderRuleSettingAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderRuleSettingAppService"></param>
        public OrderRuleController(IOrderRuleSettingAppService orderRuleSettingAppService)
        {
            _orderRuleSettingAppService = orderRuleSettingAppService;
        }

        [HttpGet]
        public void OrderRuleSettingAPI() { }

        /// <summary>
        /// 预包装订单绑定
        /// </summary>
        /// <param name="preOrderRuleDto"></param>
        /// <returns></returns>
        [HttpPost, Route("OrderBindingByPreOrderRule")]
        public CommonResponse OrderBindingByPreOrderRule(MQOrderRuleDto mqOrderRuleDto)
        {
            return _orderRuleSettingAppService.OrderBindingByPreOrderRule(mqOrderRuleDto);
        }

        /// <summary>
        /// 预包装订单绑定
        /// </summary>
        /// <param name="preOrderRuleDto"></param>
        /// <returns></returns>
        [HttpPost, Route("OutboundOrderAutomaticAllocation")]
        public CommonResponse OutboundOrderAutomaticAllocation(MQOrderRuleDto mqOrderRuleDto)
        {
            return _orderRuleSettingAppService.OutboundOrderAutomaticAllocation(mqOrderRuleDto);
        }
    }
}