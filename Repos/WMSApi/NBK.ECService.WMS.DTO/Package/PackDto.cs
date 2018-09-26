using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public partial class PackDto: BaseDto
    {
        public Guid? SysId { get; set; }
        public string PackCode { get; set; }
        public string Descr { get; set; }
        public Guid? FieldUom01 { get; set; }
        public int? FieldValue01 { get; set; }
        public bool? Cartonize01 { get; set; }
        public bool? Replenish01 { get; set; }
        public bool? InLabelUnit01 { get; set; }
        public bool? OutLabelUnit01 { get; set; }
        public Guid? FieldUom02 { get; set; }
        public int? FieldValue02 { get; set; }
        public bool? Cartonize02 { get; set; }
        public bool? Replenish02 { get; set; }
        public bool? InLabelUnit02 { get; set; }
        public bool? OutLabelUnit02 { get; set; }
        public Guid? FieldUom03 { get; set; }
        public int? FieldValue03 { get; set; }
        public bool? Cartonize03 { get; set; }
        public bool? Replenish03 { get; set; }
        public bool? InLabelUnit03 { get; set; }
        public bool? OutLabelUnit03 { get; set; }

        public bool? QueryLabelUnit01 { get; set; }

        public bool? QueryLabelUnit02 { get; set; }

        public bool? QueryLabelUnit03 { get; set; }
        public string UPC01 { get; set; }
        public string UPC02 { get; set; }
        public string UPC03 { get; set; }
        public Guid? FieldUom04 { get; set; }
        public int? FieldValue04 { get; set; }
        public string UPC04 { get; set; }
        public Guid? FieldUom05 { get; set; }
        public int? FieldValue05 { get; set; }
        public string UPC05 { get; set; }
        public string Source { get; set; }

        public DateTime UpdateDate { get; set; }

        public long UpdateBy { get; set; }

        public string UpdateUserName { get; set; }

    }
}
