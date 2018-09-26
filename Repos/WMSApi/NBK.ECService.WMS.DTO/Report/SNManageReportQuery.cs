using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.Report
{
    public class SNManageReportQuery : BaseQuery
    {
        public string SN { get; set; }

        public string PurchaseOrder { get; set; }
        public string OutboundOrder { get; set; }
        public int Status { get; set; }
        public DateTime? PurchaseDateFrom { get; set; }
        public DateTime? PurchaseDateTo { get; set; }
        public DateTime? OutboundDateFrom { get; set; }
        public DateTime? OutboundDateTo { get; set; }
    }

    public class SNManageReportDto
    {
        public Guid SysId { get; set; }

        public string SN { get; set; }

        public string PurchaseOrder { get; set; }

        public string OutboundOrder { get; set; }

        public int Status { get; set; }

        public string DisplayStatus { get { return ((ReceiptSNStatus)Status).ToDescription(); } }

        public DateTime? PurchaseDate { get; set; }

        public string DisplayPurchaseDate { get { return PurchaseDate.HasValue ? PurchaseDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty; } }

        public DateTime? OutboundDate { get; set; }

        public string DisplayOutboundDate { get { return OutboundDate.HasValue ? OutboundDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty; } }

        public string ConsigneeName { get; set; }

        public string ConsigneePhone { get; set; }

        public string ConsigneeAddress { get; set; }
    }
}
