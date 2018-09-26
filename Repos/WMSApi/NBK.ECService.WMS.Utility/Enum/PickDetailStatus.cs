using System.ComponentModel;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum PickDetailStatus
    {
        /// <summary>
        /// 缺货
        /// </summary>
        [Description("缺货")]
        OutOfStock = -1,

        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        New = 10,

        /// <summary>
        /// 拣货完成
        /// </summary>
        [Description("拣货完成")]
        Finish = 50,

        /// <summary>
        /// 取消
        /// </summary>
        [Description("取消")]
        Cancel = -999
    }
}