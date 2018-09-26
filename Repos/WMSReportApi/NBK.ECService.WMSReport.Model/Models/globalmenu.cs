using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    public class globalmenu : SysIdEntity
    {
        public string MenuName { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string ICons { get; set; }
        public Nullable<System.Guid> ParentSysId { get; set; }
        public bool IsActive { get; set; }
        public int SortSequence { get; set; }
        public string GroupMenuController { get; set; }
        public string AuthKey { get; set; }
    }
}
