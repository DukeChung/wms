using Abp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class service_station_receipt : SysIdEntity
    {
        public string ServiceStationName { get; set; }
        public decimal ReceiptPeriod { get; set; }
        public System.DateTime LastReceiptDate { get; set; }
        public decimal LastReceiptPeriodSkuQty { get; set; }
        public decimal LastReceiptPeriodQty { get; set; }
        public Nullable<System.DateTime> NextReceiptPeriodDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<decimal> Longitude { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public string OutboundOrder { get; set; }
        public Nullable<int> Status { get; set; }
        public decimal TotalQty { get; set; }
    }
}
