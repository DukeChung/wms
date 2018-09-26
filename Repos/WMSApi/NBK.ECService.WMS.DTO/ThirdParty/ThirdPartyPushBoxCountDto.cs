using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartyPushBoxCountDto
    {
        /// <summary>
        /// 外部单据号
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 总箱数
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 出库单SYSID，WMS系统记录日志时用
        /// </summary>
        public Guid OutboundSysId { get; set; }
        /// <summary>
        ///  出库单号，WMS系统记录日志时用
        /// </summary>
        public string OutboundOrder { get; set; }
        /// <summary>
        ///  操作人ID
        /// </summary>
        public int CurrentUserId { get; set; }
        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string EditUserName { get; set; }
        /// <summary>
        /// 订单类型，可参考TMSOrderType中的枚举
        /// </summary>
        public int OrderType { get; set; }
    }
}
