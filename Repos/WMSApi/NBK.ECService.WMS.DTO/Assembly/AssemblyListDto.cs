using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class AssemblyListDto
    {
        public Guid SysId { get; set; }

        public string AssemblyOrder { get; set; }

        public Guid SkuSysId { get; set; }

        public string SkuName { get; set; }

        public int Status { get; set; }

        public string StatusText { get { return ((AssemblyStatus)Status).ToDescription(); } }

        public int PlanQty { get; set; }

        public int ActualQty { get; set; }

        public DateTime? PlanProcessingDate { get; set; }

        public string PlanProcessingDateText { get { return PlanProcessingDate.HasValue ? PlanProcessingDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public DateTime? PlanCompletionDate { get; set; }

        public string PlanCompletionDateText { get { return PlanCompletionDate.HasValue ? PlanCompletionDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public DateTime? ActualProcessingDate { get; set; }

        public string ActualProcessingDateText { get { return ActualProcessingDate.HasValue ? ActualProcessingDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public DateTime? ActualCompletionDate { get; set; }

        public string ActualCompletionDateText { get { return ActualCompletionDate.HasValue ? ActualCompletionDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public long CreateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateUserName { get; set; }

        public long UpdateBy { get; set; }

        public DateTime UpdateDate { get; set; }

        public string UpdateUserName { get; set; }

        public string ExternalOrder { get; set; }
        public string Channel { get; set; }
    }
}
