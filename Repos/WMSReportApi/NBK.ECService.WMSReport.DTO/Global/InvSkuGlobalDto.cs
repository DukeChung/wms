using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class InvSkuGlobalDto
    {
        public Guid? SysId { get; set; }
        public string SkuOtherId { get; set; }
        public string SkuName { get; set; }
        public string UPC { get; set; }
        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }
        public string WarehouseName { get; set; }
    }
}
