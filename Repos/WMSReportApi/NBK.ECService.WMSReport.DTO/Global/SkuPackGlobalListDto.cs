using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class SkuPackGlobalListDto
    {
        public string SkuCode { get; set; }
        public string SkuName { get; set; }
        public string UPC { get; set; }
        public string OtherId { get; set; }

        public string UPC02 { get; set; }
        public string UOMCode02 { get; set; }
        public int? FieldValue02 { get; set; }

        public string UPC03 { get; set; }
        public string UOMCode03 { get; set; }
        public int? FieldValue03 { get; set; }

        public string UPC04 { get; set; }
        public string UOMCode04 { get; set; }
        public int? FieldValue04 { get; set; }

        public string UPC05 { get; set; }
        public string UOMCode05 { get; set; }
        public int? FieldValue05 { get; set; } 
    }
}
