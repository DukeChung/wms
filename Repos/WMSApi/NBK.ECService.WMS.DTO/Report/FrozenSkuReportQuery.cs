using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class FrozenSkuReportQuery : BaseQuery
    {
        public string UPC { get; set; }

        public string SkuName { get; set; }

        public bool IsStoreZero { get; set; }
    }
}
