using NBK.ECService.WMSReport.DTO.Base;
using System;

namespace NBK.ECService.WMSReport.DTO
{
    public class FinanceInvoicingGlobalQueryDto: BaseQuery
    {
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

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 仓库ID
        /// </summary>
        public Guid SearchWarehouseSysId { get; set; }
    }
}
