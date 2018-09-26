using Abp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class vendor_distribution : SysIdEntity
    {
        public vendor_distribution()
        {
            this.vendor_distribution_detail = new List<vendor_distribution_detail>();
        }
        
        public Nullable<System.Guid> VendorSysId { get; set; }
        public string VendorName { get; set; }
        public decimal PurcharPeriod { get; set; }
        public System.DateTime LastOrderDate { get; set; }
        public System.DateTime LastDistributionDate { get; set; }
        public decimal LastDistributionSkuQty { get; set; }
        public decimal LastDistributionQty { get; set; }
        public Nullable<System.DateTime> NextDistributionDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<decimal> Longitude { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public string PurcharOrder { get; set; }
        public Nullable<int> Status { get; set; }
        public decimal TotalQty { get; set; }
        public virtual ICollection<vendor_distribution_detail> vendor_distribution_detail { get; set; }
    }
}
