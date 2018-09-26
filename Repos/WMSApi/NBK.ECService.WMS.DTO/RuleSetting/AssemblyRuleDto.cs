using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class AssemblyRuleDto : BaseDto
    {
        public Guid? SysId { get; set; }
        /// <summary>
        /// 是否开启
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        ///    按批次属性匹配
        /// </summary>
        public bool MatchingLotAttr { get; set; }

        /// <summary>
        /// 出库排序规则
        /// </summary>
        public int DeliverySortRules { get; set; }

        /// <summary>
        /// 外借按渠道库存匹配
        /// </summary>
        public bool MatchingSkuBorrowChannel { get; set; }
    }
}
