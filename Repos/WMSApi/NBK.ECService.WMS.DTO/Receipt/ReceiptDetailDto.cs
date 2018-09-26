using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class ReceiptDetailDto
    {
        public Guid SysId { get; set; }

        public Guid ReceiptSysId { get; set; }

        public Guid SkuSysId { get; set; }

        public int? ExpectedQty { get; set; }

        public decimal? DisplayExpectedQty { get; set; }

        public int? ReceivedQty { get; set; }

        public decimal? DisplayReceivedQty { get; set; }

        public int? RejectedQty { get; set; }

        public decimal? DisplayRejectedQty { get; set; }

        public string Remark { get; set; }

        public Guid? UOMSysId { get; set; }

        public Guid? PackSysId { get; set; }

        public string ToLot { get; set; }

        public string ToLoc { get; set; }

        public string ToLpn { get; set; }
    }
}
