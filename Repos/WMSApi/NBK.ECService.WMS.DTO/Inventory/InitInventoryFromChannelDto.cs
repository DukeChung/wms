using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class InitInventoryFromChannelDto
    {
        public string WarehouseId { get; set; }
        public string SkuOtherID { get; set; }

        public string Channel { get; set; }

        public int Qty { get; set; }

        public Guid WarehouseSysId { get; set; }

        public Guid SkuSysId { get; set; }


    }

    public class InitInventoryFromChannelRequest : BaseDto
    {
        public List<InitInventoryFromChannelDto> InitList { get; set; }
    }
}
