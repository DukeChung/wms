using System.ComponentModel;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum ReceiptType
    {
        /// <summary>
        /// 采购订单
        /// </summary>
        [Description("采购订单")]
        PO = 1,

        /// <summary>
        /// 退货入库
        /// </summary>
        [Description("退货入库")]
        Return = 2,

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
        /// 农资
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