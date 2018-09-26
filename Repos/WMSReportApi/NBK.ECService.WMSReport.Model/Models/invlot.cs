using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class invlot : SysIdEntity
    {
        public System.Guid WareHouseSysId { get; set; }
        public string Lot { get; set; }
        public System.Guid SkuSysId { get; set; }
        public int CaseQty { get; set; }
        public int InnerPackQty { get; set; }
        public int Qty { get; set; }
        public int AllocatedQty { get; set; }
        public int PickedQty { get; set; }
        public int FrozenQty { get; set; }
        public int HoldQty { get; set; }
        public int Status { get; set; }
        public decimal Price { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string LotAttr01 { get; set; }
        public string LotAttr02 { get; set; }
        public string LotAttr04 { get; set; }
        public string LotAttr03 { get; set; }
        public string LotAttr05 { get; set; }
        public string LotAttr06 { get; set; }
        public string LotAttr07 { get; set; }
        public string LotAttr08 { get; set; }
        public string LotAttr09 { get; set; }
        public Nullable<System.DateTime> ReceiptDate { get; set; }
        public Nullable<System.DateTime> ProduceDate { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public string ExternalLot { get; set; }
        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }
    }
}
