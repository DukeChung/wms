using ServiceStack.DataAnnotations;

namespace NBK.ECService.WMS.Utility.Enum.Log
{
    public enum BusinessName
    {
        #region 入库
        /// <summary>
        /// 收货
        /// </summary>
        [Description("收货")]
        Receipt = 101,

        /// <summary>
        /// 上架
        /// </summary>
        [Description("上架")]
        Shelves = 102,

        /// <summary>
        /// 快速入库
        /// </summary>
        [Description("快速入库")]
        QuicklyInbound = 103,
        #endregion

        #region 出库
        /// <summary>
        /// 分配
        /// </summary>
        [Description("分配")]
        Allocate = 201,

        /// <summary>
        /// 拣货
        /// </summary>
        [Description("拣货")]
        Pick = 202,

        /// <summary>
        /// 发货
        /// </summary>
        [Description("发货")]
        Deliver = 203,

        /// <summary>
        /// 快速发货
        /// </summary>
        [Description("快速发货")]
        QuicklyDeliver = 204,

        /// <summary>
        /// 快速出库
        /// </summary>
        [Description("快速出库")]
        QuicklyOutbound = 205,

        /// <summary>
        /// 拣货
        /// </summary>
        [Description("取消拣货")]
        CancelPick = 206,

        /// <summary>
        /// 分配发货
        /// </summary>
        [Description("分配发货")]
        AllocationDeliver = 207,

        #endregion

        #region 盘点
        /// <summary>
        /// 初盘
        /// </summary>
        [Description("初盘")]
        Initial = 301,

        /// <summary>
        /// 复盘
        /// </summary>
        [Description("复盘")]
        Replay = 302,
        #endregion

        #region 调整
        /// <summary>
        /// 损益
        /// </summary>
        [Description("损益")]
        Losses = 401,

        /// <summary>
        /// 库存转移
        /// </summary>
        [Description("库存转移")]
        Transfer = 402,

        /// <summary>
        /// 库存移动
        /// </summary>
        [Description("库存移动")]
        Movement = 403,
        #endregion

        #region 增值
        /// <summary>
        /// 生产加工单
        /// </summary>
        [Description("生产加工单")]
        Assembly = 501,

        /// <summary>
        /// 商品外借
        /// </summary>
        [Description("商品外借")]
        SubBorrow = 502,
        #endregion

        #region 基础数据
        /// <summary>
        /// 承运商
        /// </summary>
        [Description("承运商")]
        Carrier = 601,

        /// <summary>
        /// 容器
        /// </summary>
        [Description("容器")]
        Container = 602,

        /// <summary>
        /// 货位
        /// </summary>
        [Description("货位")]
        Location = 603,
        #endregion 
    }
}