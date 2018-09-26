using NBK.ECService.WMSReport.DTO.Base;
using System;

namespace NBK.ECService.WMSReport.DTO
{
    public class AdjustmentDetailGlobalQuery: BaseQuery
    {
        public string AdjustmentLevelCode { get; set; }

        public DateTime? CreateDateFrom { get; set; }

        public DateTime? CreateDateTo { get; set; }

        public Guid SearchWarehouseSysId { get; set; }
    }
}
