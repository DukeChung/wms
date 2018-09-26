using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class AssemblySkuDto 
    {
        public Guid SysId { get; set; }

        public Guid SkuSysId { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }
    }

    public class AssemblySkuQuery : BaseQuery
    {
        public string SkuName { get; set; }

        public string UPC { get; set; }
    }
}
