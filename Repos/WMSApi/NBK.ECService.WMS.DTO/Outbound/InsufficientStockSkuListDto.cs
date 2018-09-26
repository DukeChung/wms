using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    /// <summary>
    /// 库存不足商品列表
    /// </summary>
    public class InsufficientStockSkuListDto
    {
        public Guid SkuSysId { get; set; }

        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundOrder { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string SkuCode { get; set; }

        /// <summary>
        /// UPC
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 出库数量
        /// </summary>
        public int OutboundQty { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public int StockQty { get; set; }

        /// <summary>
        /// 差异数量
        /// </summary>
        public int DiffQty { get; set; }

        /// <summary>
        /// 分配数量
        /// </summary>
        public int AllocatedQty { get; set; }

        /// <summary>
        /// 拣货数量
        /// </summary>
        public int PickedQty { get; set; }

        /// <summary>
        /// 冻结数量
        /// </summary>
        public int FrozenQty { get; set; }
    }
}
