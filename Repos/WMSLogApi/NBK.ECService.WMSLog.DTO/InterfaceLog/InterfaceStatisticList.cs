using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class InterfaceStatisticList : SummaryLogDto
    {
        public string InterfaceType { get; set; }

        public int RequestLength { get; set; }
        public int ResponseLength { get; set; }

    }

    public class CountList
    {
        public int CountNumber { get; set; }
    }
}
