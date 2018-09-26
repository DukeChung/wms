using NBK.ECService.WMSReport.DTO.Base;
using System;

namespace NBK.ECService.WMSReport.DTO
{
    public class FrozenSkuGlobalQuery: BaseQuery
    {
        public string UPC { get; set; }

        public string SkuName { get; set; }

        public Guid SearchWarehouseSysId { get; set; }

        public bool IsStoreZero { get; set; }
    }
}
