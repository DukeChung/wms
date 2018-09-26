using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class FlotChartSummaryDto
    {
        public FlotChartDto Inbound { get; set; }

        public FlotChartDto Outbound { get; set; }
    }

    public class FlotChartDto
    {
        public string[] Dates { get; set; }

        public int[] Recent { get; set; }

        public int[] MOM { get; set; }

        public decimal MOMPercentage { get; set; }
    }

    public class FlotChartSummaryList
    {
        public DateTime CreateDate { get; set; }
        public int? Qty { get; set; }
        public int? ReceivedQty { get; set; }
    }
}
