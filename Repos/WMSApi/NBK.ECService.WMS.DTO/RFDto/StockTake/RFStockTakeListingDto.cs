using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFStockTakeListingDto
    {
        /// <summary>
        /// 商品合计
        /// </summary>
        public int TotalSkuNumber { get; set; }

        /// <summary>
        /// 初盘清单
        /// </summary>
        public List<StockTakeFirstListDto> StockTakeFirstListDto { get; set; }

        /// <summary>
        /// 复盘清单
        /// </summary>
        public List<StockTakeSecondListDto> StockTakeSecondListDto { get; set; }
    }
}
