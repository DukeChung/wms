using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrintStockTakeReportDto
    {
        public List<PrintStockTakeReportDetailDto> PrintStockTakeReportDetails { get; set; }
    }
}
