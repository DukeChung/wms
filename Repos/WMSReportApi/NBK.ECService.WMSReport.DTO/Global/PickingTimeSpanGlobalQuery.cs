using NBK.ECService.WMSReport.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class PickingTimeSpanGlobalQuery: BaseQuery
    {
        public Guid SearchWarehouseSysId { get; set; }

        public string OutboundOrder { get; set; }

        public string Operator { get; set; }

        public DateTime? StartTimeFrom { get; set; }

        public DateTime? StartTimeTo { get; set; }

        public DateTime? EndTimeFrom { get; set; }

        public DateTime? EndTimeTo { get; set; }

    }
}
