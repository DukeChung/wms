using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO.Chart
{
    public class OutboundAndReturnCharDto
    {
        public int DisplayOrder { get; set; }

        public DateTime Date { get; set; }

        public int OutboundTotalCount { get; set; }

        public int ReturnTotalCount { get; set; }
    }
}
