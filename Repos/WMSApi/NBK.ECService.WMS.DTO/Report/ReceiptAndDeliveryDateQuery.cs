using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class ReceiptAndDeliveryDateQuery : BaseQuery
    {
        /// <summary>
        /// 入库单号
        /// </summary>
        public string PurchaseOrder { get; set; }
        public int? Type { get; set; }
        /// <summary>
        /// 出库单号
        /// </summary>
        public string ReceiptOrder { get; set; }
        /// <summary>
        /// 创建时间从
        /// </summary>
        public DateTime? CreateDateFrom { get; set; }
        /// <summary>
        /// 创建时间到
        /// </summary>
        public DateTime? CreateDateTo { get; set; }
        /// <summary>
        /// 审核时间从
        /// </summary>
        public DateTime? AuditingDateFrom { get; set; }
        /// <summary>
        /// 审核时间到
        /// </summary>
        public DateTime? AuditingDateTo { get; set; }
    }
}
