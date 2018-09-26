using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Newtonsoft.Json;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class outbounddetail : SysIdEntity
    {
        public Nullable<System.Guid> OutboundSysId { get; set; }
        public System.Guid SkuSysId { get; set; }
        public int Status { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public Nullable<System.Guid> UOMSysId { get; set; }
        public Nullable<System.Guid> PackSysId { get; set; }
        public string Loc { get; set; }
        public string Lot { get; set; }
        public string Lpn { get; set; }
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
        public Nullable<int> Qty { get; set; }
        public Nullable<int> ShippedQty { get; set; }
        public Nullable<int> PickedQty { get; set; }
        public Nullable<int> AllocatedQty { get; set; }
        public Nullable<decimal> Price { get; set; }
        [JsonIgnore]
        public virtual outbound outbound { get; set; }
        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }
        public string PackFactor { get; set; }
        public bool IsGift { get; set; }
        public int GiftQty { get; set; }

        public int ReturnQty { get; set; }
        public string Memo { get; set; }
    }
}
