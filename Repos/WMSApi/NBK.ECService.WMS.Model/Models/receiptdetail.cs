using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Newtonsoft.Json;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class receiptdetail : SysIdEntity
    { 
        public System.Guid ReceiptSysId { get; set; }
        public System.Guid SkuSysId { get; set; }
        public int Status { get; set; }
        public Nullable<int> ExpectedQty { get; set; }
        public Nullable<int> ReceivedQty { get; set; }
        public Nullable<int> RejectedQty { get; set; }
        public Nullable<decimal> Price { get; set; }

        public string Remark { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public Nullable<System.Guid> UOMSysId { get; set; }
        public Nullable<System.Guid> PackSysId { get; set; }
        public string ToLoc { get; set; }
        public string ToLot { get; set; }
        public string ToLpn { get; set; }
        public string LotAttr01 { get; set; }
        public string LotAttr02 { get; set; }
        public string LotAttr04 { get; set; }
        public string LotAttr03 { get; set; }
        public string LotAttr05 { get; set; }
        public string LotAttr06 { get; set; }
        public string LotAttr07 { get; set; }
        public string LotAttr08 { get; set; }
        public string LotAttr09 { get; set; }
        public string ExternalLot { get; set; }
        public Nullable<System.DateTime> ProduceDate { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }

        public int ShelvesStatus { get; set; }

        public int ShelvesQty { get; set; }
        public System.Guid TS { get; set; }

        public bool IsMustLot { get; set; }

        public bool IsDefaultLot { get; set; }

        [JsonIgnore]
        public virtual receipt receipt { get; set; }
    }
}
