using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFPackDto
    {
        public Guid? SysId { get; set; }
        public string PackCode { get; set; }
        public Guid? FieldUom01 { get; set; }
        public int? FieldValue01 { get; set; }
        public bool? InLabelUnit01 { get; set; }
        public Guid? FieldUom03 { get; set; }
        public int? FieldValue03 { get; set; }
        public string UPC01 { get; set; }
        public string UPC03 { get; set; }
    }
}
