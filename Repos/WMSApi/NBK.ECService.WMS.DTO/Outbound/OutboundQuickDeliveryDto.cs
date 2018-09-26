using System;
using System.Collections.Generic;

namespace NBK.ECService.WMS.DTO.Outbound
{
    public class OutboundQuickDeliveryDto : BaseDto
    {
        public Guid? SysId { get; set; }

        public string OutboundOrder { get; set; }

        public List<string> SNList { get; set; }
    }
}