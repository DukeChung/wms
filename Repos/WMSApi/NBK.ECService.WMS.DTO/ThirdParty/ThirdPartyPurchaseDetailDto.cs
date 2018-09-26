using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
 
    public class ThirdPartyPurchaseDetailDto
    {
        /// <summary>
        /// 外部ID，SkuID
        /// </summary>
        public string OtherSkuId { get; set; }

        /// <summary>
        /// 采购数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 已收货数量
        /// </summary>
        public int ReceivedQty { get; set; }

        /// <summary>
        /// 拒绝数量
        /// </summary>
        public int RejectedQty { get; set; }

        /// <summary>
        /// 本次采购价, Price
        /// </summary>
        public decimal? PurchasePrice { get; set; }

        /// <summary>
        /// Remark
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 包装系数
        /// </summary>
        public string PackFactor { get; set; }

        /// <summary>
        /// 正常品采购量
        /// </summary>
        public int NormalQty { get; set; }

        /// <summary>
        /// 赠品采购量
        /// </summary>
        public int GiftQty { get; set; }
    }
}
