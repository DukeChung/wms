using System;
using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.MQ.OrderRule;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IOrderRuleSettingAppService : IApplicationService
    {
        /// <summary>
        /// 出库规则绑定
        /// </summary>
        /// <returns></returns>
        CommonResponse OrderBindingByPreOrderRule(MQOrderRuleDto mqOrderRuleDto);

        /// <summary>
        /// 出库自动分配
        /// </summary>
        /// <returns></returns>
        CommonResponse OutboundOrderAutomaticAllocation(MQOrderRuleDto mqOrderRuleDto);

        /// <summary>
        /// 获取预包装规则
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PreOrderRuleDto GetPreOrderRuleByWarehouseSysId(Guid warehouseSysId);

        /// <summary>
        /// 保存预包装规则
        /// </summary>
        /// <param name="preOrderRuleDto"></param>
        void SavePreOrderRule(PreOrderRuleDto preOrderRuleDto);

        /// <summary>
        /// 获取出库单规则
        /// </summary>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        OutboundRuleDto GetOutboundRuleByWarehouseSysId(Guid warehouseSysId);

        WorkRuleDto GetWorkRuleByWarehouseSysId(Guid warehouseSysId);

        void SaveWorkRule(WorkRuleDto workRuleDto);

        /// <summary>
        /// 获取出库单规则
        /// </summary>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        void SaveOutboundRule(OutboundRuleDto outboundRuleDto);

        /// <summary>
        /// 获取加工规则
        /// </summary>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        AssemblyRuleDto GetAssemblyRuleWarehouseSysId(Guid warehouseSysId);


        /// <summary>
        /// 保存生产加工规则
        /// </summary>
        /// <param name="dto"></param>
        void SaveAssemblyRule(AssemblyRuleDto dto);

    }
}