using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;

namespace NBK.ECService.WMSReport.DTO.Other
{
    public class PurchaseReturnListDto : PurchaseListDto
    {
        public string ExpressNumber { get; set; }

        public string ServiceStationName { get; set; }
    }
}
