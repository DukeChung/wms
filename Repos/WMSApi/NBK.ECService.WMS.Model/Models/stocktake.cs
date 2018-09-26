using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class stocktake : SysIdEntity
    {
        public stocktake()
        {
            this.stocktakedetails = new List<stocktakedetail>();
        }
         
        public string StockTakeOrder { get; set; }
        public int Status { get; set; }
        public int StockTakeType { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<int> AssignBy { get; set; }
        public string AssignUserName { get; set; }
        public Nullable<int> ReplayBy { get; set; }
        public string ReplayUserName { get; set; }
        public System.Guid WarehouseSysId { get; set; }
        public Nullable<System.Guid> ZoneSysId { get; set; }
        public string StartLoc { get; set; }
        public string EndLoc { get; set; }
        public Nullable<System.Guid> SkuClassSysId1 { get; set; }
        public Nullable<System.Guid> SkuClassSysId2 { get; set; }
        public Nullable<System.Guid> SkuClassSysId3 { get; set; }
        public Nullable<System.Guid> SkuClassSysId4 { get; set; }
        public Nullable<System.Guid> SkuClassSysId5 { get; set; }

        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
        public virtual ICollection<stocktakedetail> stocktakedetails { get; set; }
    }
}
