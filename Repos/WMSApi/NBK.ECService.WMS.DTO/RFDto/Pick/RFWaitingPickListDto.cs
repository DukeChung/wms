using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFWaitingPickListDto
    {
        /// <summary>
        /// 出库单主键
        /// </summary>
        public Guid SysId { get; set; }

        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundOrder { get; set; }

        /// <summary>
        /// Sku数
        /// </summary>
        public int SkuCount { get; set; }

        /// <summary>
        /// 待拣货商品数量
        /// </summary>
        public int? SkuQty { get; set; }
    }
}
