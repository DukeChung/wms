using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFInventoryQuery : BaseQuery
    {
        /// <summary>
        /// 收货单号
        /// </summary>
        public string ReceiptOrder { get; set; }
        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public int UserId { get; set; }

        public Guid? SkuSysId { get; set; }
    }
}
