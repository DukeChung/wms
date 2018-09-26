using System.ComponentModel;

namespace NBK.ECService.WMSReport.Utility.Enum
{
    public enum PurchaseType
    {
        /// <summary>
        /// 采购入库
        /// </summary>
        [Description("采购入库")]
        Purchase = 0, 

        /// <summary>
        /// 负采购（采购退货）
        /// </summary>
        [Description("退货入库")]
        Return = 10,

        /// <summary>
        /// 原材料
        /// </summary>
        [Description("原材料入库 ")]
        Material = 20,

        /// <summary>
        /// 移仓
        /// </summary>
        [Description("移仓入库")]
        TransferInventory = 30,

        /// <summary>
        /// 化肥
        /// </summary>
        [Description("农资入库")]
        Fertilizer = 40,

        /// <summary>
        /// 快进快出
        /// </summary>
        [Description("快进快出")]
        FIFO = 50,

    }
}
