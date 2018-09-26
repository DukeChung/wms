using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFScanDeliveryDto
    {
        public List<Guid> vanningSysIds { get; set; }

        public string currentUserName { get; set; }

        public int currentUserId { get; set; }

        public Guid wareHouseSysId { get; set; }
    }
}
