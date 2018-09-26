namespace NBK.ECService.WMS.DTO
{
    public class VanningQueryDto : BaseQuery
    {
        public string VanningOrderSearch { get; set; }

        public string OutboundOrderSearch { get; set; }

        /// <summary>
        /// 拣货单号
        /// </summary>
        public string PickDetailOrderSearch { get; set; }

        /// <summary>
        /// 出库单状态
        /// </summary>
        public int? OutboundStatusSearch { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuNameSearch { get; set; }

        /// <summary>
        /// 商品UPC
        /// </summary>
        public string SkuUPCSearch { get; set; }
    }
}