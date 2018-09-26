using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.DTO.Base;

namespace NBK.ECService.WMSReport.DTO.Query
{
    public class PurchaseReturnQuery: PurchaseQuery
    {
        public string ExpressNumber { get; set; }

        public string ServiceStationName { get; set; }

    }
}
