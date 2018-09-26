using Abp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class sku_sales : SysIdEntity
    {
        public System.Guid SkuSysId { get; set; }
        public string SkuName { get; set; }
        public string UPC { get; set; }
        public Nullable<bool> IsAssemblyParts { get; set; }
        public Nullable<int> ReceiptQty { get; set; }
        public Nullable<int> OutboundQty { get; set; }
        public Nullable<decimal> MinPrice { get; set; }
        public Nullable<decimal> WeightedMeanPrice { get; set; }
        public Nullable<decimal> MaxPrice { get; set; }
        public Nullable<decimal> AveragePrice { get; set; }
    }
}
