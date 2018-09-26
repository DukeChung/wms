using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFGenerateContainerPickingDetailDto : BaseDto
    {
        public string OutboundOrder { get; set; }
        public string StorageCase { get; set; }
        public string UPC { get; set; }
        public Guid SkuSysId { get; set; }
        public string Loc { get; set; }
        public int PickedQty { get; set; }
    }
}
