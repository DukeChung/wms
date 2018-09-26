using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class nextnumbergen: SysIdEntity
    {
        public string KeyName { get; set; }
        public string ColumnName { get; set; }
        public string Descr { get; set; }
        public string AlphaPrefix { get; set; }
        public Nullable<int> NextNumber { get; set; }
        public string AlphaSuffix { get; set; }
        public string LeadingZeros { get; set; }
        public Nullable<int> TotalLength { get; set; }
        public string IsDateBased { get; set; }
        public string IsRefToTable { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public Nullable<int> StartNumber { get; set; }
        public string DateFormat { get; set; }
        public string IsIncreaseLength { get; set; }
        public string IsResetYear { get; set; }
        public string LetterPrefix { get; set; }
        public string LetterSuffix { get; set; }
        public string IsResetMonth { get; set; }
    }
}
