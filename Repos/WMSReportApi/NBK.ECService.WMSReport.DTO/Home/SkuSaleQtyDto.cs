using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    /// <summary>
    /// 滞销，畅销top商品
    /// </summary>
    public class SkuSaleQtyDto
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }
        /// <summary>
        /// 出库数量
        /// </summary>
        public long OutboundQty { get; set; }

        /// <summary>
        /// 库龄天数
        /// </summary>
        public long Days { get; set; }
    }
}
