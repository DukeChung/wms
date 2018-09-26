using System.ComponentModel;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum OutboundStatus
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        New = 10,

        /// <summary>
        /// 部分分配
        /// </summary>
        [Description("部分分配")]
        PartAllocation = 20,

        /// <summary>
        /// 分配完成
        /// </summary>
        [Description("分配完成")]
        Allocation = 30,

        /// <summary>
        /// 部分拣货
        /// </summary>
        [Description("部分拣货")]
        PartPick = 35,

        /// <summary>
        /// 拣货完成
        /// </summary>
        [Description("拣货完成")]
        Picking =40,

        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        Delivery = 70,

        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Cancel = -999,

        /// <summary>
        /// 关闭
        /// </summary>
        [Description("关闭")]
        Close = -10
    }
}