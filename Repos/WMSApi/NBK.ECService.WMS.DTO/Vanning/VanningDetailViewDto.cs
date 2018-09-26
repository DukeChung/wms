using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class VanningDetailViewDto : VanningDetailDto
    {
        public string CarrierName { get; set; }

        public DateTime? HandoverCreateDate { get; set; }

        public string HandoverCreateDateText { get { return HandoverCreateDate.HasValue ? HandoverCreateDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty; } }

        public string VanningOrderNumber { get; set; }

        public int MaxContainerNumber { get; set; }
    }
}
