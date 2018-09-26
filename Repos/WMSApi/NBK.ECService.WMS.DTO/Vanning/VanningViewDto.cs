using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;

namespace NBK.ECService.WMS.DTO
{
    public class VanningViewDto
    {
        public string VanningOrder { get; set; }

        public int? VanningType { get; set; }

        public string VanningTypeText { get { return VanningType.HasValue ? ((VanningType)VanningType).ToDescription() : string.Empty; } }

        public int? Status { get; set; }

        public string StatusText { get { return Status.HasValue ? ((VanningStatus)Status.Value).ToDescription() : string.Empty; } }

        public string OutboundOrder { get; set; }

        public DateTime? VanningDate { get; set; }

        public string VanningDateText { get { return VanningDate.HasValue ? VanningDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty; } }

        public Pages<VanningDetailViewDto> VanningDetailViewDtoList { get; set; }
    }
}
