using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class skuclass : SysIdEntity
    {
        public string SkuClassName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.Guid> ParentSysId { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }

        public string OtherId { get; set; }
        public string Source { get; set; }
        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }
    }
}
