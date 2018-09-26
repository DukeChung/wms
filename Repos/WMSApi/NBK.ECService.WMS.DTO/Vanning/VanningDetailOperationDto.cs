using System;
using System.Collections.Generic;

namespace NBK.ECService.WMS.DTO
{
    public class VanningDetailOperationDto: VanningDetailDto
    {
        public Guid? OutboundSysId { get; set; }

        public List<VanningPickDetailDto> VanningPickDetailDto { get; set; }
    }
}