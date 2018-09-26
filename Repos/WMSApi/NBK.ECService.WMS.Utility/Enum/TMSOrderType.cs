using System.ComponentModel;


namespace NBK.ECService.WMS.Utility.Enum
{
    /// <summary>
    /// TMS交互订单类型
    /// </summary>
    public enum TMSOrderType
    {
        /// <summary>
        /// B2B订单
        /// </summary>
        [Description("B2B订单")]
        B2BOrder = 10,

        /// <summary>
        /// 移仓订单
        /// </summary>
        [Description("移仓订单")]
        TransferOrder = 20
    }
}
