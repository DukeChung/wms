using NBK.ECService.WMS.DTO.ThirdParty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class OutboundReturnDto : BaseDto
    {
        public string ExternOrderId { get; set; }

        public Guid? OutboundSysId { get; set; }

        public string OutboundOrder { get; set; }

        public List<ThirdPartyReturnPurchaseDetailDto> OutboundReturnDetailDtoList { get; set; }
    }
    
}
