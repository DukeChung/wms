using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Newtonsoft.Json;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class purchasedetail : SysIdEntity
    {
        public System.Guid PurchaseSysId { get; set; }
        public System.Guid SkuSysId { get; set; }
        public Nullable<System.Guid> SkuClassSysId { get; set; }
        public string UomCode { get; set; }
        public System.Guid UOMSysId { get; set; }
        public Nullable<System.Guid> PackSysId { get; set; }
        public string PackCode { get; set; }
        public int GiftQty { get; set; }
        public int Qty { get; set; }
        public int ReceivedQty { get; set; }
        public int RejectedQty { get; set; }
        public Nullable<decimal> LastPrice { get; set; }
        public Nullable<decimal> HistoryPrice { get; set; }
        public Nullable<decimal> PurchasePrice { get; set; }
        public string Remark { get; set; }
        public string OtherSkuId { get; set; }
        public string PackFactor { get; set; }
        [JsonIgnore]
        public virtual purchase purchase { get; set; }
    }
}
