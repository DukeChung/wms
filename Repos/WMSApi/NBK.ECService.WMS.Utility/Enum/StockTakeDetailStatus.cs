using System.ComponentModel;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum StockTakeDetailStatus
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        New =10,

        /// <summary>
        /// 初盘
        /// </summary>
        [Description("初盘")]
        StockTake =20,

        /// <summary>
        /// 复盘
        /// </summary>
        [Description("复盘")]
        Replay =30,

        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Finish =50
    }
}