using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.Outbound
{
    public class UpdateOutboundDetailDto
    {
        public Guid SysId { get; set; }

        public int Qty { get; set; }
    }
}
