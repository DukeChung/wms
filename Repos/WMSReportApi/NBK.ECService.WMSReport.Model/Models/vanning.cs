using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class vanning : SysIdEntity
    {
        public vanning()
        {
            this.vanningdetails = new List<vanningdetail>();
        }
        public string VanningOrder { get; set; }
        public Nullable<System.Guid> OutboundSysId { get; set; }
        public Nullable<int> VanningType { get; set; }
        public Nullable<int> Status { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> VanningDate { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }
        public Guid WarehouseSysId { get; set; }
        public virtual ICollection<vanningdetail> vanningdetails { get; set; }
    }
}
