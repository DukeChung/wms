using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class DocDetailDto
    {
        public Guid SysId { get; set; }

        public Guid SkuSysId { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }

        public Guid? UOMSysId { get; set; }
        public string UOMCode { get; set; }

        public Guid? PackSysId { get; set; }

        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }
    }
}
