using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class skuborrow: SysIdEntity
    {
        public skuborrow()
        {
            this.skuborrowdetails = new List<skuborrowdetail>();
        } 
        public System.Guid WareHouseSysId { get; set; } 
        public string BorrowOrder { get; set; } 
        public int Status { get; set; }
        public string AuditingBy { get; set; }
        public string AuditingName { get; set; } 
        public Nullable<System.DateTime> AuditingDate { get; set; }
        public Nullable<System.DateTime> BorrowStartTime { get; set; }
        public Nullable<System.DateTime> BorrowEndTime { get; set; } 
        public int IsDamage { get; set; } 
        public string Remark { get; set; }
        public string BorrowName { get; set; }
        public string LendingDepartment { get; set; }
        public string OtherId { get; set; }
        public string Channel { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string CreateUserName { get; set; } 
        public string UpdateUserName { get; set; }
        public Guid TS { get; set; }
        public virtual ICollection<skuborrowdetail> skuborrowdetails { get; set; }
    }
}
