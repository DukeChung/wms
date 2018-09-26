using System;

namespace NBK.ECService.WMS.DTO.Outbound
{
    public class OutboundDetailBatchDto
    {
        public Guid? SysId { get; set; }
        public Guid SkuSysId { get; set; }
        public int? Qty { get; set; }
    }
}