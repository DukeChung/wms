using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.DTO.Base;

namespace NBK.ECService.WMSReport.DTO
{
    public class InvSkuLotGlobalDto : InvSkuLotReportDto
    {
        public string WarehouseName { get; set; } 
    }

    public class InvSkuLotGlobalQuery : InvSkuLotReportQuery
    {
        public Guid SearchWarehouseSysId { get; set; } 
    }
}
