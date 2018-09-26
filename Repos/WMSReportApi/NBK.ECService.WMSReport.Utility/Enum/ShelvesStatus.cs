using System.ComponentModel;

namespace NBK.ECService.WMSReport.Utility.Enum
{
    public enum ShelvesStatus
    {
        /// <summary>
        /// 未上架
        /// </summary>
        [Description("未上架")]
        NotOnShelves = 0,

        /// <summary>
        /// 上架中
        /// </summary>
        [Description("上架中")]
        Shelves = 10,

        /// <summary>
        /// 上架完成
        /// </summary>
        [Description("上架完成")]
        Finish = 20,
    }
}