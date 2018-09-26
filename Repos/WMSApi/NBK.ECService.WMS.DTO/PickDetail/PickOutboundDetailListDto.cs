using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PickOutboundDetailListDto
    {
        /// <summary>
        /// 出库单Id
        /// </summary>
        public Guid? OutboundSysId { get; set; }

        /// <summary>
        /// 商品数
        /// </summary>
        public int? SkuTypeQty { get; set; }
    }
}
