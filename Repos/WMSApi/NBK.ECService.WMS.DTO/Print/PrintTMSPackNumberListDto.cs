using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrintTMSPackNumberListDto
    {
        public Guid SysId { get; set; }
        public Guid OutboundSysId { get; set; }
        public string OutboundOrder { get; set; }
        public int BoxNumber { get; set; }
        public string CreateUserName { get; set; }
        public string ConsigneeArea { get; set; }
        public string ServiceStationName { get; set; }
        public Nullable<Guid> PreBulkPackSysId { get; set; }
        public string PreBulkPackOrder { get; set; }
        public string StorageCase { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<int> SkuQty { get; set; }
    }
}
