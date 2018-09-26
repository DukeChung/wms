using System;

namespace NBK.ECService.WMS.DTO
{
    public class OutboundRuleDto: BaseDto
    {
        public Guid? SysId { get; set; }
        public bool? Status { get; set; }
        public bool? MatchingLotAttr { get; set; }

        /// <summary>
        /// 出库排序规则
        /// </summary>
        public int? DeliverySortRules { get; set; }

        /// <summary>
        /// 快速发货是否异步
        /// </summary>
        public bool? DeliveryIsAsyn { get; set; }

        /// <summary>
        /// 是否异步创建出库单
        /// </summary>
        public bool CreateOutboundIsAsyn { get; set; }

        /// <summary>
        /// 是否优先领料分拣货位
        /// </summary>
        public bool IsPickingSkuLoc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool AutomaticAllocation { get; set; }


    }
}