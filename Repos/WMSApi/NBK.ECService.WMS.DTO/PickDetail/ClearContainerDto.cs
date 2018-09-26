using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class ClearContainerDto : BaseDto
    {
        public List<Guid> ContainerSysIds { get; set; }
    }
}
