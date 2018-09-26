using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class ProgressBarSummaryDto
    {
        public ProgressBarDto Purchase { get; set; }

        public ProgressBarDto PurchaseReturn { get; set; }

        public ProgressBarDto Outbound { get; set; }

        public ProgressBarDto OutboundReturn { get; set; }
    }

    public class ProgressBarDto
    {
        public int TotalCount { get; set; }

        public int Finished { get; set; }

        public int Unfinished { get { return TotalCount - Finished; } }

        public int FinishedPercentage { get { return TotalCount == 0 ? 0 : (int)Math.Floor(Finished * 100d / TotalCount); } }
    }
}
