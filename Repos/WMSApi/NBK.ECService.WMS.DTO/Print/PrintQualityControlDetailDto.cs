using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrintQualityControlDetailDto
    {
        public string SkuName { get; set; }
        public string UPC { get; set; }
        public decimal? DisplayQty { get; set; }
        public string UOMCode { get; set; }
        public string Descr { get; set; }
    }
}
