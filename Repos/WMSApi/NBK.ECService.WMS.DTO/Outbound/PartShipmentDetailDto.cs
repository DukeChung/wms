using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PartShipmentDetailDto : BaseDto
    {
        public Guid SysId { get; set; }

        public Guid SkuSysId { get; set; }

        public string SkuUPC { get; set; }

        public string SkuName { get; set; }

        public int Qty { get; set; }

        public int PickedQty { get; set; }

        public string Memo { get; set; }
    }
}
