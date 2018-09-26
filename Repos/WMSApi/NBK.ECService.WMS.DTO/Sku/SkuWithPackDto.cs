using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class SkuWithPackDto
    {
        public Guid SysId { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public string UPC { get; set; }

        public string TypeDisplay { get; set; }

        public int PackQty { get; set; }
    }

    public class DuplicateUPCChooseQuery
    {
        public string UPC { get; set; }
        public string SkuName { get; set; }
        public string SkuCode { get; set; }

        public List<string> ExcludeSkuSysId { get; set; }

        public string ExcludeSkuSysIdString { get; set; }
    }
}
