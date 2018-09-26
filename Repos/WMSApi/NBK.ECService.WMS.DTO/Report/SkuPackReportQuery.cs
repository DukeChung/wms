using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class SkuPackReportQuery : BaseQuery
    {
        public string SkuName { get; set; }
        public string SkuCode { get; set; }
        public string UPC { get; set; }
        public string PackCode { get; set; }
        public string UPC03 { get; set; }
        public string OtherId { get; set; }
    }
}
