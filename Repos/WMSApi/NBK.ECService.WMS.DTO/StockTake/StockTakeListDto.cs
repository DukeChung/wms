using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockTakeListDto
    {
        public Guid SysId { get; set; }

        public string StockTakeOrder { get; set; }

        public int StockTakeType { get; set; }

        public string StockTakeTypeText { get { return ((Utility.Enum.StockTakeType)StockTakeType).ToDescription(); } }

        public int Status { get; set; }

        public string StatusText { get { return ((Utility.Enum.StockTakeStatus)Status).ToDescription(); } }

        public DateTime? StartTime { get; set; }

        public string StartTimeText { get { return StartTime.HasValue ? StartTime.Value.ToString(PublicConst.DateTimeFormat) : string.Empty; } }

        public DateTime? EndTime { get; set; }

        public string EndTimeText { get { return EndTime.HasValue ? EndTime.Value.ToString(PublicConst.DateTimeFormat) : string.Empty; } }

        public int? AssignBy { get; set; }

        public string AssignUserName { get; set; }

        public string ReplayUserName { get; set; }

        public int? CreateBy { get; set; }

        public string CreateUserName { get; set; }

        public DateTime CreateDate { get; set; }

        public int? TotalSkuQty { get; set; }

        public int? TotalQty { get; set; }

        public bool? IsAdj { get; set; }

        public string IsAdjText { get { return IsAdj.HasValue ? (IsAdj.Value ? "是" : "否") : string.Empty; } }
    }
}
