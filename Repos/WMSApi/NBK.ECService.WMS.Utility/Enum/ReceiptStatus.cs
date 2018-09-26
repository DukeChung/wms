using System.ComponentModel;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum ReceiptStatus
    {
        /// <summary>
        /// 初始
        /// </summary>
        [Description("初始")]
        Init =0,

        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        New = 10,

        /// <summary>
        /// 已经打印
        /// </summary>
        [Description("已经打印")]
        Print =20,

        /// <summary>
        /// 收货中
        /// </summary>
        [Description("收货中")]
        Receiving =30,

        /// <summary>
        /// 收货完成
        /// </summary>
        [Description("收货完成")]
        Received =40,

        /// <summary>
        /// 取消
        /// </summary>
        [Description("取消")]
        Cancel = -999

    }
}