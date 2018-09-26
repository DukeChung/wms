using NBK.ECService.WMSReport.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class OutboundHandleDateStatisticsQuery : BaseQuery
    {
        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundOrder { get; set; }
        /// <summary>
        /// 出库单类型
        /// </summary>
        public int? OutboundType { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 下单时间从
        /// </summary>
        public DateTime? CreateDateFrom { get; set; }
        /// <summary>
        /// 下单时间到
        /// </summary>
        public DateTime? CreateDateTo { get; set; }
        /// <summary>
        /// 出库时间从
        /// </summary>
        public DateTime? ActualShipDateFrom { get; set; }
        /// <summary>
        /// 出库时间到
        /// </summary>
        public DateTime? ActualShipDateTo { get; set; }
    }
}
