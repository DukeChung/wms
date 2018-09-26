using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class adjustment : SysIdEntity
    {
        public adjustment()
        {
            this.adjustmentdetails = new List<adjustmentdetail>();
        }

        public string AdjustmentOrder { get; set; }
        public System.Guid WareHouseSysId { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public string SourceType { get; set; }
        public string SourceOrder { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
        public Nullable<System.DateTime> AuditingDate { get; set; }
        public string AuditingBy { get; set; }
        public string AuditingName { get; set; }
        public virtual ICollection<adjustmentdetail> adjustmentdetails { get; set; }
    }
}
