using System;

namespace NBK.ECService.WMS.DTO.Outbound
{
    public class UpdateOutboundDto : BaseDto
    {
        public Guid SysId { get; set; }

        public int Qty { get; set; }

        public int Status { get; set; }

        public bool PartialShipmentFlag { get; set; }
    }
}