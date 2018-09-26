using System;

namespace NBK.ECService.WMS.DTO
{
    public class DeliveryHandoverGroupDto
    {
        public Guid VanningDetailSysId { get; set; }
        public Guid? CarrierSysId { get; set; }
        public string HandoverGroupOrder { get; set; }

    }
}