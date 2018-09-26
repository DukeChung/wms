using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO.Other
{
    public class PurchaseDetailDto
    {
        public Guid? SysId { get; set; }
        public Guid? PurchaseSysId { get; set; }
        public Guid? SkuSysId { get; set; }
        public string SkuClassSysId { get; set; }

        public Guid? PackSysId { get; set; }
        public string PackCode { get; set; }
        public Guid? UomSysId { get; set; }
        public string UomCode { get; set; }
        public int? Qty { get; set; }
        public int? ReceivedQty { get; set; }
        public int? RejectedQty { get; set; }

        public decimal? DisplayQty { get; set; }
        public decimal? DisplayReceivedQty { get; set; }
        public decimal? DisplayRejectedQty { get; set; }

        public decimal? LastPrice { get; set; }
        public decimal? HistoryPrice { get; set; }
        public decimal? PurchasePrice { get; set; }

        public string Remark { get; set; }

        public string OtherSkuId { get; set; }
    }
}
