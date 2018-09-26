using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class stocktransfer : SysIdEntity
    {
        public string StockTransferOrder { get; set; }
        public System.Guid WareHouseSysId { get; set; }
        public int Status { get; set; }
        public string Descr { get; set; }
        public System.Guid FromSkuSysId { get; set; }
        public System.Guid ToSkuSysId { get; set; }
        public string FromLot { get; set; }
        public string ToLot { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public int FromQty { get; set; }
        public int ToQty { get; set; }
        public string FromLpn { get; set; }
        public string ToLpn { get; set; }
        public string FromLotAttr01 { get; set; }
        public string FromLotAttr02 { get; set; }
        public string FromLotAttr04 { get; set; }
        public string FromLotAttr03 { get; set; }
        public string FromLotAttr05 { get; set; }
        public string FromLotAttr06 { get; set; }
        public string FromLotAttr07 { get; set; }
        public string FromLotAttr08 { get; set; }
        public string FromLotAttr09 { get; set; }
        public Nullable<System.DateTime> FromProduceDate { get; set; }
        public Nullable<System.DateTime> FromExpiryDate { get; set; }
        public string FromExternalLot { get; set; }
        public string ToLotAttr01 { get; set; }
        public string ToLotAttr02 { get; set; }
        public string ToLotAttr04 { get; set; }
        public string ToLotAttr03 { get; set; }
        public string ToLotAttr05 { get; set; }
        public string ToLotAttr06 { get; set; }
        public string ToLotAttr07 { get; set; }
        public string ToLotAttr08 { get; set; }
        public string ToLotAttr09 { get; set; }
        public Nullable<System.DateTime> ToProduceDate { get; set; }
        public Nullable<System.DateTime> ToExpiryDate { get; set; }
        public string ToExternalLot { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
    }
}
