using Abp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class service_station_receipt_detail : SysIdEntity
    {
        public System.Guid ServiceStationReceiptSysId { get; set; }
        public string OutboundSysId { get; set; }
        public string OutboundOrder { get; set; }
        public Nullable<int> SkuQty { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public string CreateByName { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Status { get; set; }
        public System.Guid WareHouseSysId { get; set; }
        public string WareHouseName { get; set; }
        public Nullable<int> OutboundType { get; set; }
        public Nullable<decimal> OutboundDate { get; set; }
    }
}
