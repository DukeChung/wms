using System.ComponentModel;

namespace NBK.ECService.WMSReport.Utility.Enum
{
    public enum PreBulkPackStatus
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        New = 10,

        /// <summary>
        /// 封箱中
        /// </summary>
        [Description("封箱中")]
        PrePack = 20,

        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Finish = 30

    }
}