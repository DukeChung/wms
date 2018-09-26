using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class picture : SysIdEntity
    {
        public string TableKey { get; set; }
        public Nullable<System.Guid> TableSysId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Nullable<int> Size { get; set; }
        public string Suffix { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
    }
}
