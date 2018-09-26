using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class TMSBoxNumberDto
    {
        public string ConsigneeArea { get; set; }
        public string ServiceStationName { get; set; }
        public string ConsigneeTown { get; set; }

        public string OutboundChildType { get; set; }

        public List<OutboundPreBulkPackDto> OutboundPreBulkPackDto { get; set; }
    }
}
