using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.DTO
{
    public class WorkUserDto : BaseDto
    {
        public Guid SysId { get; set; }
        public string WorkUserCode { get; set; }
        public string WorkUserName { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? "是" : "否"; } }
        public int? WorkType { get; set; }
        public string WorkTypeText { get { return WorkType.HasValue ? ((Utility.Enum.UserWorkType)WorkType).ToDescription() : string.Empty; } }
        public int WorkStatus { get; set; }
        public string WorkStatusText { get { return ((Utility.Enum.UserWorkStatus)WorkStatus).ToDescription(); } }
        public decimal? Proficiency { get; set; }
        public Guid? TS { get; set; }
        public bool IsAssigned { get; set; }
        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public long UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
    }
}
