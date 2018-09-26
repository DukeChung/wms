using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class ReturnPurchaseTypePieDto
    {
        /// <summary>
        /// 总入库数量
        /// </summary>
        public long PurchaseQty { get; set; }
        /// <summary>
        /// 采购入库
        /// </summary>
        public double CGPurchasePie { get; set; }
        /// <summary>
        /// 原材料入库
        /// </summary>
        public double MaterialPurchasePie { get; set; }
        /// <summary>
        /// 移仓入库
        /// </summary>
        public double MovePurchasePie { get; set; }
        /// <summary>
        /// 农资入库
        /// </summary>
        public double FertilizerPurchasePie { get; set; }
        /// <summary>
        /// 退货入库
        /// </summary>
        public double ReturnPurchasePie { get; set; }

    }
    public class PurchaseTypePieDto
    {
        public long Qty { get; set; }
        public int Type { get; set; }
    }
}
