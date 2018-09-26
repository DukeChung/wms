using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Newtonsoft.Json;


namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class skuborrowdetail : SysIdEntity
    {
        public Nullable<System.Guid> SkuBorrowSysId { get; set; }
        public Nullable<System.Guid> SkuSysId { get; set; }
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
        public Nullable<System.DateTime> BorrowStartTime { get; set; }
        public Nullable<System.DateTime> BorrowEndTime { get; set; }
        public int Status { get; set; }
        public int IsDamage { get; set; }
        public string DamageReason { get; set; }
        public int ReturnQty { get; set; }
        public Guid TS { get; set; }

        [JsonIgnore]
        public virtual skuborrow skuborrow { get; set; }
    }
}
