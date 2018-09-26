using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class BatchTMSBoxNumberDto : BaseDto
    {
        public List<int> BoxNumberList { get; set; }
        /// <summary>
        /// 出库单Id
        /// </summary>
        public Guid OutboundSysId { get; set; }
        /// <summary>
        /// 出库单单号
        /// </summary>
        public string OutboundOrder { get; set; }

        public int BoxCount { get; set; }
    }
}
