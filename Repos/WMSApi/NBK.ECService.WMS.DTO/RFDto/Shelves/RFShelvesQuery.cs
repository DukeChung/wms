using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFShelvesQuery : BaseQuery
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
        /// 商品编码
        /// </summary>
        public string SkuCode { get; set; }

        public Guid? SkuSysId { get; set; }
    }
}
