using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class DiffReceiptInvOut
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid SkuSysId { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string SkuCode { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// 入库数量
        /// </summary>
        public decimal InvQty { get; set; }
        /// <summary>
        /// 库存数量
        /// </summary>
        public decimal StockQty { get; set; }

        /// <summary>
        /// 出库数量
        /// </summary>
        public decimal OutboundQty { get; set; }
        /// <summary>
        /// 差异数量
        /// </summary>
        public decimal DifferentQty { get; set; }
    }
}
