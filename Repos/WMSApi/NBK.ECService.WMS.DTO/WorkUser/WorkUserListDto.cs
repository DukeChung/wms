using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class WorkUserListDto : BaseDto
    {
        public Guid SysId { get; set; }
        public string WorkUserCode { get; set; }
        public string WorkUserName { get; set; }
    }
}
