using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class FrozenSkuDto
    {
        public Guid SkuSysId { get; set; }

        public string UPC { get; set; }

        public string SkuName { get; set; }

        public string Loc { get; set; }

        public int AllocatedQty { get; set; }

        public int PickedQty { get; set; }

        public int Qty { get; set; }

        public Guid ZoneSysId { get; set; }

        public string Lot { get; set; }

        public string LotAttr01 { get; set; }
    }
}
