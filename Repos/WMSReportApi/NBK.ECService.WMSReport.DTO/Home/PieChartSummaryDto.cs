using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class PieChartSummaryDto
    {
        public PieChartDto ReceiptOrder { get; set; }

        public PieChartDto PickOrder { get; set; }

        public PieChartDto NotOutPCT { get; set; }

        public PieChartDto NotInPCT { get; set; }
    }

    public class PieChartDto
    {
        public int TotalCount { get; set; }

        public int Finished { get; set; }

        public int Unfinished { get { return TotalCount - Finished; } }

        public int FinishedPercentage { get { return TotalCount == 0 ? 0 : (int)Math.Floor(Finished * 100d / TotalCount); } }
    }
}
