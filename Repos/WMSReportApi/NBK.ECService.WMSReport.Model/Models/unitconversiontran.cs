using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class unitconversiontran : SysIdEntity
    {
        public System.Guid WareHouseSysId { get; set; }
        public string DocOrder { get; set; }
        public System.Guid DocSysId { get; set; }
        public System.Guid DocDetailSysId { get; set; }
        public System.Guid SkuSysId { get; set; }
        public string TransType { get; set; }

        public string SourceTransType { get; set; }
        public decimal FromQty { get; set; }
        public int ToQty { get; set; }
        public string Loc { get; set; }
        public string Lot { get; set; }
        public string Lpn { get; set; }
        public string Status { get; set; }
        public System.Guid PackSysId { get; set; }
        public string PackCode { get; set; }
        public System.Guid FromUOMSysId { get; set; }
        public System.Guid ToUOMSysId { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
    }
}
