using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFOutboundReviewDiffDto
    {
        /// <summary>
        /// 已复核
        /// </summary>
        public List<RFOutboundReviewDetailDto> ReviewedList { get; set; }

        /// <summary>
        /// 待复核
        /// </summary>
        public List<RFOutboundReviewDetailDto> ToReviewList { get; set; }

        /// <summary>
        /// 复核差异
        /// </summary>
        public List<RFOutboundReviewDetailDto> ReviewDiffList { get; set; }
    }
}
