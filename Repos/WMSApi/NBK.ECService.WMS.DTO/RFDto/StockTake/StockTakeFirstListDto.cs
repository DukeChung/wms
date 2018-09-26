using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockTakeFirstListDto
    {
        /// <summary>
        /// 盘点明细主键
        /// </summary>
        public Guid SysId { get; set; }

        /// <summary>
        /// 商品主键
        /// </summary>
        public Guid SkuSysId { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string SkuCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 货位
        /// </summary>
        public string Loc { get; set; }

        /// <summary>
        /// 初盘数量
        /// </summary>
        public int StockTakeQty { get; set; }

        public decimal? DisplayStockTakeQty { get; set; }
    }
}
