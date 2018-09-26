using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFWaitingReviewDto
    {
        public RFWaitingReviewDto()
        {
            this.WaitingReviewList = new List<RFOutboundDetailDto>();
            this.IsCached = false;
        }

        public List<RFOutboundDetailDto> WaitingReviewList { get; set; }

        public bool IsCached { get; set; }
    }
}
