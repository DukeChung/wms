using System;

namespace NBK.ECService.WMS.DTO
{
    public class ScanDeliveryDto : BaseDto
    {
        public string VanningOrder { get; set; }
        public Guid OutboundSysId { get; set; }
        public Guid VanningSysId { get; set; }
        public Guid CarrierSysId { get; set; }
        public string CarrierName { get; set; }
        public string ContainerNumber { get; set; }
        public string CarrierNumber { get; set; }
        public decimal Weight { get; set; }

        public int? OutboundStatus { get; set; }

    }
}