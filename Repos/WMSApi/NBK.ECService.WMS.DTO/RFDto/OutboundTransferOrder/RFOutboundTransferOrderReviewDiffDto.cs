using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFOutboundTransferOrderReviewDiffDto
    {
        public Guid SkuSysId { get; set; }
        /// <summary>
        /// 商品明细
        /// </summary>
        public string SkuName { get; set; }
        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }
        /// <summary>
        /// 出库数量
        /// </summary>
        public decimal OutboundQty { get; set; }
        /// <summary>
        /// 复核数量
        /// </summary>
        public decimal ReviewQty { get; set; }
    }
}
