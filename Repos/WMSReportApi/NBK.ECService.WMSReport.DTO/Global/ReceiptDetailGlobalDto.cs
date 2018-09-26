using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class ReceiptDetailGlobalDto: ReceiptDetailReportDto
    {
        public string WarehouseName { get; set; }
    }

    public class ReceiptDetailGlobalQuery : ReceiptDetailReportQuery
    {
        public Guid SearchWarehouseSysId { get; set; }
    }
}
