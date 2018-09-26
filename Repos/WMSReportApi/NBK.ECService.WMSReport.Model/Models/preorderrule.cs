using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class preorderrule : SysIdEntity
    {
        public Nullable<bool> Status { get; set; }
        public Nullable<int> MatchingRate { get; set; }

        public Nullable<int> MatchingMaxRate { get; set; }

        public Nullable<bool> MatchingSku { get; set; }
        public Nullable<bool> MatchingQty { get; set; }
        public Nullable<bool> DeliveryIntercept { get; set; }
        public Nullable<bool> ExceedQty { get; set; }
        public System.Guid WarehouseSysId { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string CreateUserName { get; set; }
        public string UpdateUserName { get; set; }
        public Nullable<bool> ServiceStation { get; set; }
    }
}
