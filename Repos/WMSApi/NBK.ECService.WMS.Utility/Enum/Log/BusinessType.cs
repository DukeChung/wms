using ServiceStack.DataAnnotations;

namespace NBK.ECService.WMS.Utility.Enum.Log
{
    public enum BusinessType
    {
        /// <summary>
        /// 入库
        /// </summary>
        [Description("入库")]
        Inbound = 10,

        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        Outbound = 20,

        /// <summary>
        /// 盘点
        /// </summary>
        [Description("盘点")]
        Stocktake = 30,

        /// <summary>
        /// 调整
        /// </summary>
        [Description("调整")]
        Adjustment = 40,

        /// <summary>
        /// 增值
        /// </summary>
        [Description("增值")]
        VAS = 50,

        /// <summary>
        /// 基础数据
        /// </summary>
        [Description("基础数据")]
        Basedata = 60
    }
}