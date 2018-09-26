using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFSingleReviewScanningDto : BaseDto
    {
        public string StorageCase { get; set; }
        public string TransferOrder { get; set; }
        public Guid SkuSysId { get; set; }
        public string UPC { get; set; }
        public int Qty { get; set; }
        public string OutboundOrder { get; set; }
    }
}
