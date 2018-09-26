using Abp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class vendor_distribution_detail : SysIdEntity
    {
        public Nullable<System.Guid> VendorDistributionSysId { get; set; }
        public Nullable<System.Guid> PurcharSysId { get; set; }
        public string PurcharOrder { get; set; }
        public Nullable<int> SkuQty { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public string CreateByName { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Status { get; set; }
        public System.Guid WareHouseSysId { get; set; }
        public string WareHouseName { get; set; }
        public Nullable<decimal> ProcurementTime { get; set; }
        public virtual vendor_distribution vendor_distribution { get; set; }
    }
}
