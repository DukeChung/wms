using System.ComponentModel;

namespace NBK.ECService.WMSReport.Utility.Enum
{
    public enum OutboundType
    {

        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 10,

        /// <summary>
        /// B2C
        /// </summary>
        [Description("B2C")]
        B2C = 20,

        /// <summary>
        /// B2B
        /// </summary>
        [Description("B2B")]
        B2B = 30,

        /// <summary>
        /// 退货出库
        /// </summary>
        [Description("退货出库")]
        Return = 40,

        /// <summary>
        /// 快进快出
        /// </summary>
        [Description("快进快出")]
        FIFO = 50,

        /// <summary>
        /// 移仓
        /// </summary>
        [Description("移仓出库")]
        TransferInventory = 60,

        /// <summary>
        /// 原材料
        /// </summary>
        [Description("原材料出库")]
        Material = 70,

        /// <summary>
        /// 化肥
        /// </summary>
        [Description("农资出库")]
        Fertilizer = 80,

    }
}