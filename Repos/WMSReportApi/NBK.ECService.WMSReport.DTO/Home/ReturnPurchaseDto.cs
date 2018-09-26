using NBK.ECService.WMSReport.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    /// <summary>
    /// 退货入库单
    /// </summary>
    public class ReturnPurchaseDto
    {
        /// <summary>
        /// 出库单号
        /// </summary>
        public string ExternalOrder { get; set; }

        /// <summary>
        /// 出库时间
        /// </summary>
        public DateTime? OutboundDate { get; set; }
        public string OutboundDateDisplay
        {
            get
            {
                return OutboundDate.HasValue ? OutboundDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty;
            }
        }

        /// <summary>
        /// 退货时间
        /// </summary>
        public DateTime? PurchaseDate { get; set; }


        public string PurchaseDateDisplay
        {
            get
            {
                return PurchaseDate.HasValue ? PurchaseDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty;
            }
        }
    }
}
