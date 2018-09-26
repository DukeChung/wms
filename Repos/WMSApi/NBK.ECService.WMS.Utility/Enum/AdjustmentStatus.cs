using System.ComponentModel;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum AdjustmentStatus
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        New = 10,

        /// <summary>
        /// 审核通过
        /// </summary>
        [Description("审核通过")]
        Audit = 20,

        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Void = -999
    }
}