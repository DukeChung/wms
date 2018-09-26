using System.ComponentModel;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum StockTakeStatus
    {
        /// <summary>
        /// 未开始
        /// </summary>
        [Description("未开始")]
        New = 10,

        /// <summary>
        /// 已开始
        /// </summary>
        [Description("已开始")]
        Started = 15,

        /// <summary>
        /// 初盘
        /// </summary>
        [Description("初盘")]
        StockTake = 20,

        /// <summary>
        /// 初盘完成
        /// </summary>
        [Description("初盘完成")]
        StockTakeFinished = 30,

        /// <summary>
        /// 复盘
        /// </summary>
        [Description("复盘")]
        Replay = 40,

        /// <summary>
        /// 复盘完成
        /// </summary>
        [Description("复盘完成")]
        ReplayFinished = 50,

        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Finished = 60
    }
}