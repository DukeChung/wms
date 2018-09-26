using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class OutboundTransferPrintDto
    {
        public Guid SysId { get; set; }
        public string ConsigneeArea { get; set; }
        public string ServiceStationName { get; set; }
        public string ConsigneeTown { get; set; }
        public string TransferOrder { get; set; }

        public string OutboundChildType { get; set; }
        public int BoxNumber { get; set; }
        public int? OutboundType { get; set; }
        public string ToWareHouseName { get; set; }
    }
}
