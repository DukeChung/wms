using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class FRInvLotLocLpnListDto
    {
        public Guid SysId { get; set; }
        public Guid WareHouseSysId { get; set; }
        public Guid SkuSysId { get; set; }
        public string Loc { get; set; }
        public string Lot { get; set; }
        public string Lpn { get; set; }
        public int Qty { get; set; }
        public int AllocatedQty { get; set; }
        public int PickedQty { get; set; }
        public int Status { get; set; }
        public int FrozenQty { get; set; }
    }
}
