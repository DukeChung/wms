using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Utility.Message
{
    public class ClientInfo
    {
        public string ConnId { get; set; }

        public int UserId { get; set; }

        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        public string WarehouseSysId { get; set; }
    }
}
