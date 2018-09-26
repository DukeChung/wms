using System.ComponentModel;

namespace NBK.ECService.WMS.Utility.Enum
{
    public enum StockTakeType
    {
        /// <summary>
        /// 区域盘点
        /// </summary>
        [Description("按区域盘点")]
        Location = 10,

        /// <summary>
        /// 按品类盘点
        /// </summary>
        [Description("按商品盘点")]
        Sku = 20,

        /// <summary>
        /// ABC类盘点
        /// </summary>
        [Description("ABC类盘点")]
        Abc = 30,

        /// <summary>
        /// 抽查盘点
        /// </summary>
        [Description("抽查盘点")]
        Random = 40,

        /// <summary>
        /// 动碰盘点
        /// </summary>
        [Description("动碰盘点")]
        Touch = 50
    }
}