using NBK.ECService.WMSReport.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO.Report
{
    public class SNManageReportQuery : BaseQuery
    {
        public string SN { get; set; }

        public string PurchaseOrder { get; set; }
        public string OutboundOrder { get; set; }
        public int Status { get; set; }
        public DateTime? PurchaseDateFrom { get; set; }
        public DateTime? PurchaseDateTo { get; set; }
        public DateTime? OutboundDateFrom { get; set; }
        public DateTime? OutboundDateTo { get; set; }
    }
}
