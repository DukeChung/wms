using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class SkuReceiptOutboundReportDto
    {
        public string AfterDay { get; set; }

        public int ReceiptCount { get; set; }

        public int OutboundCount { get; set; }
    }

    public class SkuReceiptReportDto
    {
        public DateTime ReceiptDate { get; set; }

        public int ReceivedQty { get; set; }

        public decimal DisplayReceivedQty { get; set; }
    }

    public class SkuOutboundReportDto
    {
        public DateTime OutboundDate { get; set; }

        public int OutboundQty { get; set; }
    }
}
