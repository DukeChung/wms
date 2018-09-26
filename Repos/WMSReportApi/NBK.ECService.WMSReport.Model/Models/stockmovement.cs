using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMSReport.Model.Models
{
    [Serializable]
    public partial class stockmovement : SysIdEntity
    {
        public string StockMovementOrder { get; set; }
        public System.Guid WareHouseSysId { get; set; }
        public int Status { get; set; }
        public string Descr { get; set; }
        public System.Guid SkuSysId { get; set; }
        public string Lot { get; set; }
        public string Lpn { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public string FromQty { get; set; }
        public string ToQty { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
    }
}
