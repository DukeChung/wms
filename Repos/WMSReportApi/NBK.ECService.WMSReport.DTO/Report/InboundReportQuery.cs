using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.DTO.Base;

namespace NBK.ECService.WMSReport.DTO
{
    public class InboundReportQuery : BaseQuery
    {
        public string PurchaseOrderSearch { get; set; }

        public int? PurchaseTypeSearch { get; set; }

        public DateTime? StartDateSearch { get; set; }

        public DateTime? EndDateSearch { get; set; }
    }
}
