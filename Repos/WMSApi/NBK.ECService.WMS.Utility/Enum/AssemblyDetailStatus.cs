using System.ComponentModel;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum AssemblyDetailStatus
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
        PartAllocation = 15,

        /// <summary>
        /// 分配完成
        /// </summary>
        [Description("分配完成")]
        Allocation = 20,

        /// <summary>
        /// 部分拣货
        /// </summary>
        [Description("部分拣货")]
        PartPicking = 30,

        /// <summary>
        /// 拣货完成
        /// </summary>
        [Description("拣货完成")]
        Picking = 40,

        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Finished = 50
    }
}
