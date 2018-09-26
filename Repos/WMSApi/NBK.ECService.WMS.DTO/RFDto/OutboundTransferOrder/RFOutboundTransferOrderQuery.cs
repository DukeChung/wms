using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFOutboundTransferOrderQuery : BaseDto
    {
        public string OutboundOrder { get; set; }
        public string StorageCase { get; set; }
        public string TransferOrder { get; set; }
        public Guid SkuSysId { get; set; }
        public string UPC { get; set; }
        public decimal Qty { get; set; }
    }
}
