using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class OutboundTransferOrderMoveDto : BaseDto
    {
        /// <summary>
        /// 来源交界明细SysId
        /// </summary>
        public Guid OutboundTransferOrderDetailSyId { get; set; }

        /// <summary>
        /// 移动到交接单SysId
        /// </summary>
        public Guid ToOutboundTransferOrderSysId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid SkuSysId { get; set; }

        /// <summary>
        /// 转移商品数量
        /// </summary>
        public int MoveSkuQty { get; set; }
    }
}
