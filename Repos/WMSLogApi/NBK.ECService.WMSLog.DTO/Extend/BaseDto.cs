using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class BaseDto
    {
        public int CurrentUserId { get; set; }
        public string CurrentDisplayName { get; set; }
        public Guid WarehouseSysId { get; set; }

        public int system_id { get; set; }
    }
}
