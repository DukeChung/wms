using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class OutboundBindQuery : BaseQuery
    {
        /// <summary>
        /// 预包装单号
        /// </summary>
        public string PrePackOrder { get; set; }
        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundOrder { get; set; }
        /// <summary>
        /// /
        /// </summary>
        public Guid OutboundSysId { get; set; }
    }
}
