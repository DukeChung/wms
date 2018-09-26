using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockTakeDto
    {
        public Guid SysId { get; set; }

        public string StockTakeOrder { get; set; }

        public int Status { get; set; }

        public int StockTakeType { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int? AssignBy { get; set; }

        public string AssignUserName { get; set; }

        public Guid WarehouseSysId { get; set; }

        public Guid? ZoneSysId { get; set; }

        public string StartLoc { get; set; }

        public string EndLoc { get; set; }

        public Guid? SkuClassSysId1 { get; set; }

        public Guid? SkuClassSysId2 { get; set; }

        public Guid? SkuClassSysId3 { get; set; }

        public Guid? SkuClassSysId4 { get; set; }

        public Guid? SkuClassSysId5 { get; set; }

        public long CreateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateUserName { get; set; }

        public long UpdateBy { get; set; }

        public DateTime UpdateDate { get; set; }

        public string UpdateUserName { get; set; }
    }
}
