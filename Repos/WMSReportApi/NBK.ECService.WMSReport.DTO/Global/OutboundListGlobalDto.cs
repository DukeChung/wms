using NBK.ECService.WMSReport.DTO.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class OutboundListGlobalDto: OutboundListDto
    {
        public Guid WarehouseSysId { get; set; }

        public string WarehouseName { get; set; }
    }
}
