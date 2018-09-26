using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public class receiptdatarecord : SysIdEntity
    {
        public System.Guid WareHouseSysId { get; set; }
        public System.Guid ReceiptSysId { get; set; }
        public string ReceiptOrder { get; set; }
        public System.Guid SkuSysId { get; set; }
        public int Qty { get; set; }
        public int GiftQty { get; set; }
        public int RejectedQty { get; set; }
        public int GiftRejectedQty { get; set; }
        public int AdjustmentQty { get; set; }
        public string Remark { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string CreateUserName { get; set; }
        public string UpdateUserName { get; set; }

    }
}
