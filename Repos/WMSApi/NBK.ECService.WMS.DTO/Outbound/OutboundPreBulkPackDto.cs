using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class OutboundPreBulkPackDto
    {
        public int SkuQty { get; set; }
        public int Qty { get; set; }
        public Guid PreBulkPackSysId { get; set; }
        public string PreBulkPackOrder { get; set; }

        public int BoxNumber { get; set; }
    }
}
