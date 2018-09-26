using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class ThirdPreBullPackDto
    {
        /// <summary>
        /// 出库单SysId，WMS记录日志时使用
        /// </summary>
        public Guid? OutboundSysId { get; set; }
        /// <summary>
        /// 出库单号，WMS记录日志时使用
        /// </summary>
        public string OutboundOrder { get; set; }
        /// <summary>
        /// 外部ID
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 交接单号List
        /// </summary>
        public List<string> StorageCases { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserName { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public int CurrentUserId { get; set; }

        /// <summary>
        /// 订单类型，可参考TMSOrderType中的枚举
        /// </summary>
        public int OrderType { get; set; }
    }
}
