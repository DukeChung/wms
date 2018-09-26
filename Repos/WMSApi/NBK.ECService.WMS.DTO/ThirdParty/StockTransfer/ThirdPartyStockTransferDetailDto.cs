using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartyStockTransferDetailDto
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        public string OtherSkuId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 来源渠道
        /// </summary>
        public string FromChannel { get; set; }

        /// <summary>
        /// 目标渠道
        /// </summary>
        public string ToChannel { get; set; }

        /// <summary>
        /// 来源批次号
        /// </summary>
        public string FromBatchNumber { get; set; }

        /// <summary>
        /// 目标批次号
        /// </summary>
        public string ToBatchNumber { get; set; }
    }
}
