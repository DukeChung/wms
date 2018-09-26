using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Newtonsoft.Json;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class adjustmentdetail : SysIdEntity
    {
        public Nullable<System.Guid> AdjustmentSysId { get; set; }
        public Nullable<System.Guid> SkuSysId { get; set; }
        public string AdjustlevelCode { get; set; }
        public string Loc { get; set; }
        public string Lot { get; set; }
        public string Lpn { get; set; }
        public int Qty { get; set; }
        public string Remark { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
        [JsonIgnore]
        public virtual adjustment adjustment { get; set; }
    }
}
