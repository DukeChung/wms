using Abp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class userwarehousemapping : SysIdEntity
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public System.Guid WarehouseSysId { get; set; }
        public Nullable<long> CreateBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUserName { get; set; }
    }
}
