using NBK.ECService.WMSReport.DTO.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class OutboundGlobalQuery : OutboundQuery
    {
        public Guid SearchWarehouseSysId { get; set; }
    }
}
