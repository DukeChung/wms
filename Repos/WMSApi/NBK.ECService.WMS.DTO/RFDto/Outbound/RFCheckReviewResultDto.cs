using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFCheckReviewResultDto
    {
        public RFCheckReviewResultDto()
        {
            RFCommResult = new RFCommResult();
            Skus = new List<RFOutboundDetailDto>();
        }

        public List<RFOutboundDetailDto> Skus { get; set; }

        public RFCommResult RFCommResult { get; set; }
    }
}
