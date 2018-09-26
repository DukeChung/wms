using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class work : SysIdEntity
    {
        public string WorkOrder { get; set; }
        public int Status { get; set; }
        public int WorkType { get; set; }
        public Nullable<int> Priority { get; set; }

        public Nullable<Guid> AppointUserId { get; set; }
        public string AppointUserName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? WorkTime { get; set; }

        public string Descr { get; set; }
        public string Source { get; set; }
        public Nullable<Guid> DocSysId { get; set; }
        public string DocOrder { get; set; }
        public Nullable<Guid> DocDetailSysId { get; set; }
        public Nullable<Guid> SkuSysId { get; set; }

        public string Lot { get; set; }
        public string Lpn { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public Nullable<int> FromQty { get; set; }
        public Nullable<int> ToQty { get; set; }
        public Guid WarehouseSysId { get; set; }


        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public long UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }

    }
}
