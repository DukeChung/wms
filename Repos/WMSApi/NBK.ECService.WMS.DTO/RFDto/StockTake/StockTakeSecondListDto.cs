using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockTakeSecondListDto
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
        /// 商品描述
        /// </summary>
        public string SkuDescr { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        public string UPC01 { get; set; }

        public string UPC02 { get; set; }

        public string UPC03 { get; set; }

        public string UPC04 { get; set; }

        public string UPC05 { get; set; }

        public int? FieldValue01 { get; set; }

        public int? FieldValue02 { get; set; }

        public int? FieldValue03 { get; set; }

        public int? FieldValue04 { get; set; }

        public int? FieldValue05 { get; set; }

        /// <summary>
        /// 货位
        /// </summary>
        public string Loc { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 复盘数量
        /// </summary>
        public int? ReplayQty { get; set; }

        public decimal? DisplayReplayQty { get; set; }

        /// <summary>
        /// 初盘数量
        /// </summary>
        public int StockTakeQty { get; set; }
    }
}
