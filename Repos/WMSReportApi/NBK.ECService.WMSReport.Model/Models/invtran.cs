using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class invtran : SysIdEntity
    { 
        public System.Guid WareHouseSysId { get; set; }
        public string DocOrder { get; set; }
        public System.Guid DocSysId { get; set; }
        public System.Guid DocDetailSysId { get; set; }
        public System.Guid SkuSysId { get; set; }
        public string SkuCode { get; set; }
        public string TransType { get; set; }
        public string SourceTransType { get; set; }
        public int Qty { get; set; }
        public string Loc { get; set; }
        public string Lot { get; set; }
        public string Lpn { get; set; }
        public string ToLoc { get; set; }
        public string ToLot { get; set; }
        public string ToLpn { get; set; }
        public string Status { get; set; }
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
        public System.Guid PackSysId { get; set; }
        public string PackCode { get; set; }
        public System.Guid UOMSysId { get; set; }
        public string UOMCode { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }
    }
}
