using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO 
{
    public class OutboundDetailGlobalDto: OutboundDetailReportDto
    {
        public string WarehouseName { get; set; }
    }

    public class OutboundDetailGlobalQuery : OutboundDetailReportQuery
    {
        public Guid SearchWarehouseSysId { get; set; }
    }
}
