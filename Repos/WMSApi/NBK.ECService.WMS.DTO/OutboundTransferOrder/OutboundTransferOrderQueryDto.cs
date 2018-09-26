using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class OutboundTransferOrderQueryDto : BaseDto
    {
        public Guid OutboundSysId { get; set; }
        public int Status { get; set; }
        public int TransferType { get; set; }
    }
}
