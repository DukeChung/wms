using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFCheckReviewDetailSkuDto : BaseDto
    {
        public string OutboundOrder { get; set; }

        public Guid? SkuSysId { get; set; }

        public decimal Qty { get; set; }

        public string SkuUPC { get; set; }
    }
}
