using System.ComponentModel;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum PurchaseStatus
    {
        /// <summary>
        /// 待入库
        /// </summary>
        [Description("待入库")]
        New = 0,

        /// <summary>
        /// 收货中
        /// </summary>
        [Description("收货中")]
        InReceipt = 10,

        /// <summary>
        /// 部分入库
        /// </summary>
        [Description("部分入库")]
        PartReceipt =20,

        /// <summary>
        /// 已入库
        /// </summary>
        [Description("已入库")]
        Finish = 30,

        /// <summary>
        /// 关闭
        /// </summary>
        [Description("关闭")]
        Close = -10,

        /// <summary>
        /// 终止入库
        /// </summary>
        [Description("终止入库")]
        StopReceipt =-99,

        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Void =-999
    }
}