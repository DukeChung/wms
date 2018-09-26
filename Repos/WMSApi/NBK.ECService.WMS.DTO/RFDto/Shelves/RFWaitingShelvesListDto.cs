using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFWaitingShelvesListDto
    {
        /// <summary>
        /// 收货单主键
        /// </summary>
        public Guid SysId { get; set; }

        /// <summary>
        /// 收货单号
        /// </summary>
        public string ReceiptOrder { get; set; }

        /// <summary>
        /// Sku数
        /// </summary>
        public int SkuNumber { get; set; }

        /// <summary>
        /// 待上架商品数量
        /// </summary>
        public int? SkuQty { get; set; }

        public decimal DisplaySkuQty { get; set; }

        public Guid SkuSysId { get; set; }
    }
}
