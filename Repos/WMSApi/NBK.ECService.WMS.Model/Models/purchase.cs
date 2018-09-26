using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class purchase : SysIdEntity
    {
        public purchase()
        {
            this.purchasedetails = new List<purchasedetail>();
        }

        public string PurchaseOrder { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string ExternalOrder { get; set; }
        public System.Guid VendorSysId { get; set; }
        public string Descr { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public Nullable<System.DateTime> AuditingDate { get; set; }
        public string AuditingBy { get; set; }
        public string AuditingName { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public string Source { get; set; }
        public Nullable<System.DateTime> LastReceiptDate { get; set; }
        public string PoGroup { get; set; }
        public Nullable<System.DateTime> ClosedDate { get; set; }
        public Nullable<System.Guid> WarehouseSysId { get; set; }
        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }
        public long? UpdateBy { get; set; }
        public System.DateTime? UpdateDate { get; set; }

        public string Channel { get; set; }
        public string BatchNumber { get; set; }

        public Nullable<System.Guid> OutboundSysId { get; set; }
        public string OutboundOrder { get; set; }
        public string BusinessType { get; set; }
        public Nullable<System.Guid> FromWareHouseSysId { get; set; }
        public virtual ICollection<purchasedetail> purchasedetails { get; set; }
    }
}
