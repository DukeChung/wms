using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Newtonsoft.Json;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class pickdetail : SysIdEntity
    {
        public System.Guid WareHouseSysId { get; set; }
        public Nullable<System.Guid> OutboundSysId { get; set; }
        public Nullable<System.Guid> OutboundDetailSysId { get; set; }
        public string PickDetailOrder { get; set; }
        public System.DateTime PickDate { get; set; }
        public int Status { get; set; }
        public System.Guid SkuSysId { get; set; }
        public Nullable<System.Guid> UOMSysId { get; set; }
        public Nullable<System.Guid> PackSysId { get; set; }
        public string Loc { get; set; }
        public string Lot { get; set; }
        public string Lpn { get; set; }
        public Nullable<int> Qty { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }
        public int SourceType { get; set; }
    }
}
