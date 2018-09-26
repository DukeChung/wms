using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFOutboundDetailDto
    {
        public Guid SkuSysId { get; set; }

        public Guid? OutboundSysId { get; set; }

        public string UPC { get; set; }

        public string SkuName { get; set; }

        public int SkuQty { get; set; }

        public string UOMCode { get; set; }

        public string UPC01 { get; set; }

        public string UPC02 { get; set; }

        public string UPC03 { get; set; }

        public string UPC04 { get; set; }

        public string UPC05 { get; set; }

        public int? FieldValue01 { get; set; }

        public int? FieldValue02 { get; set; }

        public int? FieldValue03 { get; set; }

        public int? FieldValue04 { get; set; }

        public int? FieldValue05 { get; set; }

        public decimal DisplaySkuQty { get; set; }
    }
}
