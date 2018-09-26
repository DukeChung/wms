using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartyFinishQualityControlDto : BaseDto
    {
        /// <summary>
        /// 源单据号（出库单或入库单）
        /// </summary>
        public int OriginalOrderId { get; set; }

        /// <summary>
        /// 仅质检不合格商品集合 若为空，则默认质检全部通过
        /// </summary>
        public List<ThirdPartyFinishQualityControlDetailDto> QCProductRecords { get; set; }

        /// <summary>
        /// 质检时间
        /// </summary>
        public DateTime QCDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        ///// <summary>
        ///// 是否物流拒收 0 否 1 是
        ///// </summary>
        //public int IsRejection { get; set; }
    }

    public class ThirdPartyFinishQualityControlDetailDto
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductCode { get; set; }

        /// <summary>
        /// 质检不合格数量
        /// </summary>
        public int ProductQty { get; set; }

        /// <summary>
        /// 不合格原因描述
        /// </summary>
        public string Reason { get; set; }
    }
}
