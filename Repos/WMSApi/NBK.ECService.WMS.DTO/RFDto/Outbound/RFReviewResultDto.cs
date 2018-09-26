using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFReviewResultDto
    {
        public RFReviewResultDto()
        {
            this.NoScanSkus = new List<RFOutboundReviewInfo>();
            this.QtyReducedSkus = new List<RFOutboundReviewInfo>();
            this.SameSkus = new List<RFOutboundReviewInfo>();
        }

        public List<RFOutboundReviewInfo> NoScanSkus { get; set; }

        public List<RFOutboundReviewInfo> QtyReducedSkus { get; set; }

        public List<RFOutboundReviewInfo> SameSkus { get; set; }
    }
}
