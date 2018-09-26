using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{

    /// <summary>
    /// 全局收发存
    /// </summary>
    public class StockInOutListDto
    {
        /// <summary>
        /// 时间 yyyy-MM
        /// </summary>
        public string CreateDate { get; set; }
        /// <summary>
        /// 库存
        /// </summary>
        public decimal InitialQty { get; set; }
        /// <summary>
        /// 入库
        /// </summary>
        public decimal ReceiptQty { get; set; }
        /// <summary>
        /// 出库(负数)
        /// </summary>
        public decimal OutboundQty { get; set; }

        /// <summary>
        /// 出库数量（正数：只读）
        /// </summary>
        public decimal OutboundQtyDisplay
        {
            get
            {
                return Math.Abs(OutboundQty);
            }
        }

    }
}
