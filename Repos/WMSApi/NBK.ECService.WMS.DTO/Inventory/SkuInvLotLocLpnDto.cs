using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class SkuInvLotLocLpnDto
    {
        public Guid SkuSysId { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public string UPC { get; set; }

        public bool IsActive { get; set; }

        public string IsActiveText { get { return IsActive ? "是" : "否"; } }

        public string UOMCode { get; set; }

        public Guid WareHouseSysId { get; set; }

        public string WareHouseName { get; set; }
        public decimal DisplayQty { get; set; }

        public string Loc { get; set; }

        public string Lot { get; set; }

        public string Lpn { get; set; }

        public string Channel { get; set; }
    }
}
