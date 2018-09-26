using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockTakeSecondDto : BaseDto
    {
        /// <summary>
        /// 盘点单号
        /// </summary>
        public string StockTakeOrder { get; set; }

        /// <summary>
        /// 输入数量
        /// </summary>
        public decimal InputQty { get; set; }

        /// <summary>
        /// 复盘数量
        /// </summary>
        public int ReplayQty { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid? SkuSysId { get; set; }

        /// <summary>
        /// 库位
        /// </summary>
        public string Loc { get; set; }
    }
}
