using System.ComponentModel;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum WorkStatus
    {
        /// <summary>
        /// 挂起
        /// </summary>
        [Description("挂起")]
        Hang = 10,

        /// <summary>
        /// 进行中
        /// </summary>
        [Description("进行中")]
        Working = 20,

        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Finish = 30,

        /// <summary>
        /// 拒绝
        /// </summary>
        [Description("拒绝")]
        Refuse = 40,

        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Void = -99,

        /// <summary>
        /// 取消
        /// </summary>
        [Description("取消")]
        Cancel = -90

    }
}
