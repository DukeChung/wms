using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class SkuInvLotLocLpnQuery : BaseQuery
    {
        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }

        public Guid WarehouseSysId { get; set; }

        public string Loc { get; set; }

        public string Lot { get; set; }

        public string Lpn { get; set; }

        public string Channel { get; set; }
    }
}
