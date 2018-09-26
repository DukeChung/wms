using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class GetInventoryDto
    {
        public Guid SkuSysId { get; set; }

        public string Loc { get; set; }

        public string Lot { get; set; }

        public string Lpn { get; set; }

        public Guid WareHouseSysId { get; set; }
    }
}
