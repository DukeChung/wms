using System;
using System.Collections.Generic;
using System.Web.Http;
using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;

namespace NBK.ECService.WMS.ApiController.OrderRuleSettingControllers
{

    /// <summary>
    /// 订单管理
    /// </summary>
    [RoutePrefix("api/OrderRuleSetting")]
    [AccessLog]
    public class OrderRuleSettingController : AbpApiController
    {

        private IOrderRuleSettingAppService _orderRuleSettingAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderRuleSettingAppService"></param>
        public OrderRuleSettingController(IOrderRuleSettingAppService orderRuleSettingAppService)
        {
            _orderRuleSettingAppService = orderRuleSettingAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void OrderRuleSettingAPI() { }


        /// <summary>
        /// 分页获取预包装单数据
        /// </summary>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPreOrderRuleByWarehouseSysId")]
        public PreOrderRuleDto GetPreOrderRuleByWarehouseSysId(Guid warehouseSysId)
        {
            return _orderRuleSettingAppService.GetPreOrderRuleByWarehouseSysId(warehouseSysId);
        }

        /// <summary>
        /// 分页获取预包装单数据
        /// </summary>
        /// <param name="preOrderRuleDto"></param>
        /// <returns></returns>
        [HttpPost, Route("SavePreOrderRule")]
        public void SavePreOrderRule(PreOrderRuleDto preOrderRuleDto)
        {
            _orderRuleSettingAppService.SavePreOrderRule(preOrderRuleDto);
        }


        /// <summary>
        /// 分页获取预包装单数据
        /// </summary>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetOutboundRuleByWarehouseSysId")]
        public OutboundRuleDto GetOutboundRuleByWarehouseSysId(Guid warehouseSysId)
        {
            return _orderRuleSettingAppService.GetOutboundRuleByWarehouseSysId(warehouseSysId);
        }

        /// <summary>
        /// 分页获取预包装单数据
        /// </summary>
        /// <param name="outboundRuleDto"></param>
        /// <returns></returns>
        [HttpPost, Route("SaveOutboundRule")]
        public void SaveOutboundRule(OutboundRuleDto outboundRuleDto)
        {
            _orderRuleSettingAppService.SaveOutboundRule(outboundRuleDto);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetWorkRuleByWarehouseSysId")]
        public WorkRuleDto GetWorkRuleByWarehouseSysId(Guid warehouseSysId)
        {
            return _orderRuleSettingAppService.GetWorkRuleByWarehouseSysId(warehouseSysId);
        }

        /// <summary>
        /// 分页获取预包装单数据
        /// </summary>
        /// <param name="workRuleDto"></param>
        /// <returns></returns>
        [HttpPost, Route("SaveWorkRule")]
        public void SaveWorkRule(WorkRuleDto workRuleDto)
        {
            _orderRuleSettingAppService.SaveWorkRule(workRuleDto);
        }


        /// <summary>
        /// 获取加工规则
        /// </summary>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetAssemblyRuleWarehouseSysId")]
        public AssemblyRuleDto GetAssemblyRuleWarehouseSysId(Guid warehouseSysId)
        {
            return _orderRuleSettingAppService.GetAssemblyRuleWarehouseSysId(warehouseSysId);
        }


        /// <summary>
        /// 保存生产加工规则
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("SaveAssemblyRule")]
        public void SaveAssemblyRule(AssemblyRuleDto dto)
        {
            _orderRuleSettingAppService.SaveAssemblyRule(dto);
        }
    }
}