using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class transferinventory : SysIdEntity
    {
        public transferinventory()
        {
            this.transferinventorydetails = new List<transferinventorydetail>();
        }
        public string TransferInventoryOrder { get; set; }
        public System.Guid FromWareHouseSysId { get; set; }
        public string FromWareHouseName { get; set; }
        public System.Guid ToWareHouseSysId { get; set; }
        public string ToWareHouseName { get; set; }
        public Nullable<System.DateTime> TransferOutboundDate { get; set; }
        public Nullable<System.DateTime> TransferInboundDate { get; set; }
        public string TransferOutboundOrder { get; set; }
        public Nullable<System.Guid> TransferOutboundSysId { get; set; }
        public string TransferPurchaseOrder { get; set; }
        public Nullable<System.Guid> TransferPurchaseSysId { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> ExternOrderDate { get; set; }
        public string ExternOrderId { get; set; }
        public string Remark { get; set; }
        public string AuditingBy { get; set; }
        public string AuditingName { get; set; }
        public Nullable<System.DateTime> AuditingDate { get; set; }
        public long CreateBy { get; set; }
        public string CreateUserName { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public string UpdateUserName { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string ShippingMethod { get; set; }
        public Nullable<decimal> Freight { get; set; }
        public string Channel { get; set; }
        public virtual ICollection<transferinventorydetail> transferinventorydetails { get; set; }
    }
}
