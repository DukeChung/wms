using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public class outboundexception : SysIdEntity
    {
        public System.Guid OutboundSysId { get; set; }
        public System.Guid OutboundDetailSysId { get; set; }
        public int? ExceptionQty { get; set; }
        public string ExceptionReason { get; set; }
        public string ExceptionDesc { get; set; }
        public string Result { get; set; }
        public string Department { get; set; }
        public string Responsibility { get; set; }
        public string Remark { get; set; }
        public Nullable<bool> IsSettlement { get; set; }
        public long CreateBy { get; set; }
        public long UpdateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreateUserName { get; set; }
        public string UpdateUserName { get; set; }
    }
}
