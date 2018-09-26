using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class CancelWorkDto : BaseDto
    {
        public List<Guid> SysIds { get; set; }

        public List<Guid> DocSysIds { get; set; }

        public int Status { get; set; }
    }
}
