using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrintPurchaseDetailViewDto
    {
        public Guid SkuSysId { get; set; }
        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string SkuUPC { get; set; }

        public string SkuDescr { get; set; }

        public string PackCode { get; set; }

        public string UOMCode { get; set; }
        /// <summary>
        /// 包装系数
        /// </summary>
        public string PackFactor { get; set; }

        public int? Qty { get; set; }
        public int GiftQty { get; set; }

        public int? ReceivedQty { get; set; }

        public decimal? DisplayQty { get; set; }

        public decimal? DisplayReceivedQty { get; set; }

        public string Remark { get; set; }
    }
}
