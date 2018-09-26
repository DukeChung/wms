
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.Utility;

namespace NBK.ECService.WMSReport.DTO
{
    public class ReceiptDetailReportDto : BaseDto
    {
        public Guid SysId { get; set; }

        public string ReceiptOrder { get; set; }

        public string ExternalOrder { get; set; }

        public string VendorName { get; set; }

        public int Status { get; set; }

        public string StatusDisplay { get { return ((Utility.Enum.ReceiptStatus)Status).ToDescription(); } }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }
        public string SkuDescr { get; set; }

        public string UPC { get; set; }

        public int ReceivedQty { get; set; }

        public decimal DisplayShelvesQty { get; set; }

        public decimal DisplayReceivedQty { get; set; }

        public int ShelvesQty { get; set; }

        public string VendorSysId { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public string ReceivedDateDisplay { get { return ReceivedDate.HasValue ? ReceivedDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty; } }

        public DateTime? CreateDate { get; set; }

        public string OutboundOrder { get; set; }
    }

    public class ReceiptDetailReportQuery : BaseQuery
    {
        public string VendorName { get; set; }

        public string ReceiptOrder { get; set; }

        public string ExternalOrder { get; set; }

        public int? Status { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }

        public DateTime? ReceivedDateFrom { get; set; }

        public DateTime? ReceivedDateTo { get; set; }

        public int? ShelvesStatus { get; set; }

        public int? ReceiptType { get; set; }

        public bool? IsMaterial { get; set; }

        public string OutboundOrder { get; set; }
    }
}
