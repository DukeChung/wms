using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class LogStatisticBaseDto
    {
        public int PastDays = 7;

        public LogStatisticBaseDto()
        {
            this.PastDaysCount = new int[PastDays];
        }

        public int StatisticCount { get; set; }

        public decimal? AvgResponseTime { get; set; }

        public string AvgTimeText
        {
            get { return AvgResponseTime.HasValue ? AvgResponseTime.Value.ToString("f2") : "N/A"; }
        }

        public decimal? MinResponseTime { get; set; }

        public string MinTimeText
        {
            get { return MinResponseTime.HasValue ? (MinResponseTime.Value == decimal.Zero ? "0.01" : MinResponseTime.Value.ToString("f2")) : "N/A"; }
        }

        public decimal? MaxResponseTime { get; set; }

        public string MaxTimeText
        {
            get { return MaxResponseTime.HasValue ? MaxResponseTime.Value.ToString("f2") : "N/A"; }
        }

        public int[] PastDaysCount { get; set; }
    }
}
