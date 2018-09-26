using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrintStockTakeDetailDto
    {
        public string SkuCode { get; set; }

        public string SkuUPC { get; set; }

        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public string UOMCode { get; set; }

        public int StockTakeQty { get; set; }

        /// <summary>
        /// 显示数量
        /// </summary>
        public decimal DisplayStockTakeQty { get; set; }

        public int? ReplayQty { get; set; }

        /// <summary>
        /// 显示数量
        /// </summary>
        public decimal DisplayReplayQty { get; set; }

        public DateTime CreateDate { get; set; }

        public string Loc { get; set; }
    }
}
