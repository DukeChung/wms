using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class WorkUpdateDto : BaseDto
    {
        public Guid SysId { get; set; }
        public Guid? AppointUserId { get; set; }
        public string AppointUserName { get; set; }
    }
}
