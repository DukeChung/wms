using System;

namespace NBK.ECService.WMS.DTO
{
    public class VanningPickDetailOperationDto
    {
        public string OutbounOrder { get; set; }
        public Guid? OutboundSysId { get; set; }
        public Guid? SkuSysId { get; set; } 

        public string SkuName { get; set; }

        public string UPC { get; set; }

        public string SkuDescr { get; set; }

        public string ContainerNumber { get; set; }

        public decimal Weight { get; set; }

        public int Qty { get; set; }

    }
}