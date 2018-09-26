using System;
using NBK.ECService.WMS.Utility.Enum;

namespace NBK.ECService.WMS.DTO
{
    public class PurchaseReturnQuery: PurchaseQuery
    {
        public string ExpressNumber { get; set; }

        public string ServiceStationName { get; set; }
    }
}
