using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class ReceiptDetailCheckLotDto
    {
        public Guid SysId { get; set; }

        public Guid? CheckLotSysId { get; set; }

        public Guid? ReceiptSysId { get; set; }

        public string ToLot { get; set; }

        public string ToLoc { get; set; }

        public string ToLpn { get; set; }
    }
}
