using NBK.ECService.WMSReport.DTO.Base;
using System;

namespace NBK.ECService.WMSReport.DTO
{
    public class OutboundBoxGlobalQuery: BaseQuery
    {

        public string OutboundOrder { get; set; }

        public int? Status { get; set; }

        public DateTime? CreateDateStart { get; set; }

        public DateTime? CreateDateEnd { get; set; }

        public DateTime? ActualShipDateStart { get; set; }

        public DateTime? ActualShipDateEnd { get; set; }

        public Guid SearchWarehouseSysId { get; set; }
    }
}
