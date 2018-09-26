using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockTakeFirstDto : BaseDto
    {
        /// <summary>
        /// 货位
        /// </summary>
        public string Loc { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string SkuCode { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 盘点数量
        /// </summary>
        public int StockTakeQty { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 盘点单号
        /// </summary>
        public string StockTakeOrder { get; set; }

        /// <summary>
        /// 复盘数量
        /// </summary>
        public int ReplayQty { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid SkuSysId { get; set; }

        public decimal InputQty { get; set; }
    }
}
