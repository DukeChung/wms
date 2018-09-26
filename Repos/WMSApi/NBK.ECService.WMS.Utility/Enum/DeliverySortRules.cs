using System.ComponentModel;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum DeliverySortRules
    {
        /// <summary>
        /// 先生产先出库
        /// </summary>
        [Description("先生产先出库")]
        FirstProduceFirstOutbound = 10,

        /// <summary>
        /// 后生产先出库
        /// </summary>
        [Description("后生产先出库")]
        AfterProduceFirstOutbound = 20,

        /// <summary>
        /// 先入库先出库
        /// </summary>
        [Description("先入库先出库")]
        FirstReceiptFirstOutbound = 30
    }


    public enum DeliveryAssemblyRule
    {
        /// <summary>
        /// 先生产先分配
        /// </summary>
        [Description("先生产先分配")]
        FirstProduceFirstAssembly = 10,

        /// <summary>
        /// 后生产先分配
        /// </summary>
        [Description("后生产先分配")]
        AfterProduceFirstAssembly = 20,

        /// <summary>
        /// 先入库先分配
        /// </summary>
        [Description("先入库先分配")]
        FirstReceiptFirstAssembly = 30
    }
}
