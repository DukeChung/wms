using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartyTransferInventoryInboundDetailBackDto
    {
        /// <summary>
        /// 商品编码
        /// </summary>
        public int ProductNo { get; set; }

        /// <summary>
        /// 入库数量
        /// </summary>
        public int Counts { get; set; }

        /// <summary>
        /// 拒收数量
        /// </summary>
        public int RejectCounts { get; set; }
    }
}
