using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public class prebulkpack : SysIdEntity
    {
        public prebulkpack()
        {
            this.prebulkpackdetails = new List<prebulkpackdetail>();
        }

        public System.Guid WareHouseSysId { get; set; }
        public string PreBulkPackOrder { get; set; }
        public string StorageCase { get; set; }
        public Nullable<int> Status { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string CreateUserName { get; set; }
        public string UpdateUserName { get; set; }
        public Nullable<System.Guid> OutboundSysId { get; set; }
        public string OutboundOrder { get; set; }
        public virtual ICollection<prebulkpackdetail> prebulkpackdetails { get; set; }
    }
}
