using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class InboundReportDto : BaseDto
    {
        public Guid SysId { get; set; }

        public string PurchaseOrder { get; set; }

        public int PurchaseType { get; set; }

        public string PurchaseTypeText { get { return ((PurchaseType)PurchaseType).ToDescription(); } }

        public int Status { get; set; }

        public string StatusText { get { return ((PurchaseStatus)Status).ToDescription(); } }

        public DateTime? LastReceiptDate { get; set; }

        public string LastReceiptDateText { get { return LastReceiptDate.HasValue ? LastReceiptDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public int ReceivedQty { get; set; }

        public decimal DisplayReceivedQty { get; set; }

        public int RejectedQty { get; set; }

        public decimal DisplayRejectedQty { get; set; }
    }
}
