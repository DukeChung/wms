using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class InboundReportQuery : BaseQuery
    {
        public string PurchaseOrderSearch { get; set; }

        public int? PurchaseTypeSearch { get; set; }

        public DateTime? StartDateSearch { get; set; }

        public DateTime? EndDateSearch { get; set; }
    }
}
