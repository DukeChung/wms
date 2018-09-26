using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrintOutboundDetailDto
    {
        public Guid SysId { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public Guid SkuSysId { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string SkuCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string SkuDescr { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string UomCode { get; set; }

        /// <summary>
        /// 出库单数量
        /// </summary>
        public int? Qty { get; set; }

        public decimal? DisplayQty { get; set; }

        /// <summary>
        /// 发货数量
        /// </summary>
        public int? ShippedQty { get; set; }

        public decimal? DisplayShippedQty { get; set; }

        public string PackFactor { get; set; }

        public string Memo { get; set; }
    }
}
