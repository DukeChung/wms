using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class pack : SysIdEntity
    {
        public string PackCode { get; set; }
        public string Descr { get; set; }
        public Nullable<System.Guid> FieldUom01 { get; set; }
        public Nullable<int> FieldValue01 { get; set; }
        public Nullable<bool> Cartonize01 { get; set; }
        public Nullable<bool> Replenish01 { get; set; }
        public Nullable<bool> InLabelUnit01 { get; set; }
        public Nullable<bool> OutLabelUnit01 { get; set; }
        public Nullable<System.Guid> FieldUom02 { get; set; }
        public Nullable<int> FieldValue02 { get; set; }
        public Nullable<bool> Cartonize02 { get; set; }
        public Nullable<bool> Replenish02 { get; set; }
        public Nullable<bool> InLabelUnit02 { get; set; }
        public Nullable<bool> OutLabelUnit02 { get; set; }
        public Nullable<System.Guid> FieldUom03 { get; set; }
        public Nullable<int> FieldValue03 { get; set; }
        public Nullable<bool> Cartonize03 { get; set; }
        public Nullable<bool> Replenish03 { get; set; }
        public Nullable<bool> InLabelUnit03 { get; set; }
        public Nullable<bool> OutLabelUnit03 { get; set; }

        public Nullable<bool> QueryLabelUnit01 { get; set; }

        public Nullable<bool> QueryLabelUnit02 { get; set; }

        public Nullable<bool> QueryLabelUnit03 { get; set; }

        public string UPC01 { get; set; }
        public string UPC02 { get; set; }
        public string UPC03 { get; set; }

        public Nullable<System.Guid> FieldUom04 { get; set; }
        public Nullable<int> FieldValue04 { get; set; }
        public string UPC04 { get; set; }

        public Nullable<System.Guid> FieldUom05 { get; set; }
        public Nullable<int> FieldValue05 { get; set; }
        public string UPC05 { get; set; }

        public string Source { get; set; }
    }
}
