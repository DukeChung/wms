using System;

namespace NBK.ECService.WMS.DTO
{
    public class PickDetailQuery : BaseQuery
    {
        public string OutboundOrderSearch { get; set; }
        public string PickDetailOrderSearch { get; set; }
        public int? StatusSearch { get; set; }

        public string ExternOrderSearch { get; set; }
        public int? OutboundTypeSearch { get; set; }

        /// <summary>
        /// SKU数量条件符号
        /// </summary>
        public int SkuTypeCountSymbol { get; set; }

        /// <summary>
        /// SKU数量
        /// </summary>

        public string SkuTypeCountSearch { get; set; }

        /// <summary>
        /// 商品数量条件符号
        /// </summary>
        public int SkuQtyCountSymbol { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public string SkuQtyCountSearch { get; set; }

        public string CarrierNameSearch { get; set; }
        public DateTime? StartOutboundDateSearch { get; set; }
        public DateTime? EndOutboundDateSearch { get; set; }
        public string ConsigneeNameSearch { get; set; }
        public string SkuUPCSearch { get; set; }

        public string ServiceStationNameSearch { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuNameSearch { get; set; }

        /// <summary>
        /// 平台订单号
        /// </summary>
        public string PlatformOrderSearch { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string OutboundChildTypeSearch { get; set; }
    }
}