using Abp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class vendor_sku_price : SysIdEntity
    {
        public System.Guid SkuSysId { get; set; }
        public string SkuName { get; set; }
        public string SkuUPC { get; set; }
        public Nullable<System.Guid> VendorSysId { get; set; }
        public string VendorName { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.Guid> PurchaseSysId { get; set; }
        public int Qty { get; set; }
    }
}
