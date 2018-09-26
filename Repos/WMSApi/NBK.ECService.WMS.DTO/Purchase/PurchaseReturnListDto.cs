using System;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;

namespace NBK.ECService.WMS.DTO
{
    public class PurchaseReturnListDto: PurchaseListDto
    {
        public string ExpressNumber { get; set; }

        public string ServiceStationName { get; set; }
    }
}
