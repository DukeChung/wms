using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFContainerPickingListDto
    {
        public Guid SysId { get; set; }

        public string OutboundOrder { get; set; }

        public DateTime? AuditingDate { get; set; }

        public int TotalQty { get; set; }

        public int SkuQty { get; set; }
    }
}
