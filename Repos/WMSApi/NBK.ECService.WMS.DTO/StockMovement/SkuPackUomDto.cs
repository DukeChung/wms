using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class SkuPackUomDto
    {
        public Guid SkuSysId { get; set; }

        public string SkuCode { get; set; }

        public Guid? PackSysId { get; set; }

        public string PackCode { get; set; }

        public Guid? UOMSysId { get; set; }

        public string UOMCode { get; set; }
    }
}
