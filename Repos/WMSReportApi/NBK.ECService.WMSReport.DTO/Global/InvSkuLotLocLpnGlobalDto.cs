using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class InvSkuLotLocLpnGlobalDto : InvSkuLotLocLpnReportDto
    {
        public string WarehouseName { get; set; }
    }

    public class InvSkuLotLocLpnGlobalQuery : InvSkuLotLocLpnReportQuery
    {
        public Guid SearchWarehouseSysId { get; set; }
    }
}
