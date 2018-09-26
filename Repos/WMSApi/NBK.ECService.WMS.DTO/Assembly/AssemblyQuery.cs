using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class AssemblyQuery : BaseQuery
    {
        public string AssemblyOrderSearch { get; set; }

        public string SkuNameSearch { get; set; }

        public string SkuUPCSearch { get; set; }

        public int? StatusSearch { get; set; }

        public DateTime? PlanProcessingDateSearch { get; set; }

        public DateTime? PlanCompletionDateSearch { get; set; }

        public DateTime? ActualProcessingDateSearch { get; set; }

        public DateTime? ActualCompletionDateSearch { get; set; }

        public string ExternalOrderSearch { get; set; }
        public string Channel { get; set; }

        /// <summary>
        /// 待拣货加工单查询条件
        /// </summary>
        public bool WaitPickSearch { get; set; } = false;
    }
}
