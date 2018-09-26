using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{

    public class ThirdPartyOutboundDetailDto
    {
        /// <summary>
        /// 外部ID，SkuID
        /// </summary>
        public string OtherSkuId { get; set; }

        /// <summary>
        /// Qty
        /// </summary>
        public int? Qty { get; set; }

        /// <summary>
        /// 售价
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 包装系数
        /// </summary>
        public string PackFactor { get; set; }

        /// <summary>
        /// 是否含赠品
        /// </summary>
        public bool IsGift { get; set; }

        /// <summary>
        /// 赠品数量，进攻wms内部使用，对外接口不需要传值
        /// </summary>
        public int GiftQty { get; set; }

    }
}
