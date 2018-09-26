using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class InvSkuLocGlobalQuery : InvSkuLocReportQuery
    {
        public Guid SearchWarehouseSysId { get; set; }
    }
}
