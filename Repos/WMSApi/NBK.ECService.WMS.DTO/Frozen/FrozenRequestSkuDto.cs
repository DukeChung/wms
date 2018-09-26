using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class FrozenRequestSkuDto
    {
        public Guid SysId { get; set; }
        public Guid SkuSysId { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }

        public string UOMCode { get; set; }

        public Guid WareHouseSysId { get; set; }

        public string WareHouseName { get; set; }

        public int Qty { get; set; }
        public decimal DisplayQty { get; set; }

        public decimal DisplayFrozenQty { get; set; }

        public string Loc { get; set; }

        public string Lot { get; set; }

        public string Lpn { get; set; }

        public int FrozenSource { get; set; }

        public string Channel { get; set; }
    }
}
