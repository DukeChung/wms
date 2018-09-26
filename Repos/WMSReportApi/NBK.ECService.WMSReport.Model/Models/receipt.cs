using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class receipt : SysIdEntity
    {
        public receipt()
        {
            this.receiptdetails = new List<receiptdetail>();
            this.receiptsns = new List<receiptsn>();
        }

        public string ReceiptOrder { get; set; }
        //public string DisplayExternalOrder { get; set; }
        public string ExternalOrder { get; set; }
        public int ReceiptType { get; set; }
        public System.Guid WarehouseSysId { get; set; }
        public Nullable<System.DateTime> ExpectedReceiptDate { get; set; }
        public Nullable<System.DateTime> ReceiptDate { get; set; }
        public Nullable<int> Status { get; set; }
        public string Descr { get; set; }
        public string ReturnDescr { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.Guid> VendorSysId { get; set; }
        public Nullable<System.DateTime> ClosedDate { get; set; }
        public Nullable<System.DateTime> ArrivalDate { get; set; }
        public Nullable<int> TotalExpectedQty { get; set; }
        public Nullable<int> TotalReceivedQty { get; set; }
        public Nullable<int> TotalRejectedQty { get; set; }
        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }
        public System.Guid TS { get; set; }
        public string AppointUserNames { get; set; }
        public virtual ICollection<receiptdetail> receiptdetails { get; set; }
        public virtual warehouse warehouse { get; set; }
        public virtual ICollection<receiptsn> receiptsns { get; set; }
    }
}
