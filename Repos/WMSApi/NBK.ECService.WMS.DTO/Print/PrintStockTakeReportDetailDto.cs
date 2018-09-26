using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrintStockTakeReportDetailDto
    {
        public string SkuCode { get; set; }

        public string SkuUPC { get; set; }

        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public string UOMCode { get; set; }

        public int? Status { get; set; }

        public string StatusText { get { return Status.HasValue ? ((StockTakeDetailStatus)Status.Value).ToDescription() : string.Empty; } }

        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }

        public int StockTakeQty { get; set; }

        public decimal DisplayStockTakeQty { get; set; }

        public int? ReplayQty { get; set; }

        public decimal DisplayReplayQty { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
