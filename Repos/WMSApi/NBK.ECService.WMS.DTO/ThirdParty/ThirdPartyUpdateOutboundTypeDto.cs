using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class ThirdPartyUpdateOutboundTypeDto
    {
        /// <summary>
        /// 外部单据号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        public string EditUserName { get; set; }
        /// <summary>
        /// 更新人Id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 出库单SysId，用于记录日志
        /// </summary>
        public Guid? OutboundSysId { get; set; }
        /// <summary>
        /// 出库单号，用于记录日志
        /// </summary>
        public string OutboundOrder { get; set; }
        /// <summary>
        /// 订单类型，可参考TMSOrderType中的枚举
        /// </summary>
        public int OrderType { get; set; }
    }
}
