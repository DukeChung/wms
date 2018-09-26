using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class ImportStockMovement : BaseDto
    {
        public List<StockMovementImportDto> StockMovementImportDto { get; set; }
    }

    public class StockMovementImportDto
    {
        /// <summary>
        /// 导入商品外部Id
        /// </summary>
        public string OtherId { get; set; }
        /// <summary>
        /// 商品UPC
        /// </summary>
        public string UPC { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }
        /// <summary>
        /// 变更数量
        /// </summary>
        public decimal ChangerQty { get; set; }
        /// <summary>
        /// 来源货位
        /// </summary>
        public string FromLoc { get; set; }
        /// <summary>
        /// 目标货位
        /// </summary>
        public string ToLoc { get; set; }
    }
}
