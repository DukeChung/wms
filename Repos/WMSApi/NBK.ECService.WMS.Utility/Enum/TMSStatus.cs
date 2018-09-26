using System.ComponentModel;
namespace NBK.ECService.WMS.Utility
{
    /// <summary>
    /// 订单状态变更TMS交互
    /// </summary>
    public enum TMSStatus
    {

        /// <summary>
        /// 退货
        /// </summary>
        [Description("退货")]
        Return = 2,
        /// <summary>
        /// 部分退货
        /// </summary>
        [Description("部分退货")]
        PartReturn = 3,

        /// <summary>
        /// 取消发货
        /// </summary>
        [Description("取消发货")]
        Cancel = 4,

        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Close = 5,

        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        Outbound = 6
    }
}
