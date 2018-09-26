using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Newtonsoft.Json;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class stocktakedetail : SysIdEntity
    {
        public System.Guid StockTakeSysId { get; set; }
        public System.Guid SkuSysId { get; set; }
        public string Loc { get; set; }
        public string Lot { get; set; }
        public string Lpn { get; set; }
        public System.DateTime StockTakeTime { get; set; }
        public int Qty { get; set; }
        public int StockTakeQty { get; set; }
        public string Remark { get; set; }
        public Nullable<int> Status { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
        public Nullable<int> ReplayQty { get; set; }
        [JsonIgnore]
        public virtual stocktake stocktake { get; set; }
    }
}
