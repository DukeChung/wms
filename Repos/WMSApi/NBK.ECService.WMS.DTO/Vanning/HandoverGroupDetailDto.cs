using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class HandoverGroupDetailDto
    {
        public string VanningOrder { get; set; }

        public string ContainerNumber { get; set; }

        public string VanningOrderDisplay
        {
            get
            {
                return $"{VanningOrder}-{ContainerNumber}";
            }
        }

        public string CarrierNumber { get; set; }

        public decimal Weight { get; set; }

        public string ExternOrderId { get; set; }

    }
}
