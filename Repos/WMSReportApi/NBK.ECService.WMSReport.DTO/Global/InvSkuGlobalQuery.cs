using System;
using NBK.ECService.WMSReport.DTO.Base;

namespace NBK.ECService.WMSReport.DTO
{
    public class InvSkuGlobalQuery: BaseQuery
    {
        /// <summary>
        /// 大于0
        /// </summary>
        public bool? GreaterThanZero { get; set; }

        /// <summary>
        /// 商品Code
        /// </summary>
        public string SkuCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string OtherId { get; set; }

        public bool? IsMaterial { get; set; }

        public Guid SearchWarehouseSysId { get; set; }
    }
}
