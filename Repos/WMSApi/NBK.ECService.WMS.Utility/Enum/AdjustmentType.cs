using System.ComponentModel;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum AdjustmentType
    {
        /// <summary>
        /// 损益
        /// </summary>
        [Description("损益")]
        ProfiAndLoss = 10,

        /// <summary>
        /// 退货损益
        /// </summary>
        [Description("退货损益")]
        ReturnLoss = 20,

        /// <summary>
        /// 移仓损益
        /// </summary>
        [Description("移仓损益")]
        ShiftingProfitLoss = 30,
    }
}