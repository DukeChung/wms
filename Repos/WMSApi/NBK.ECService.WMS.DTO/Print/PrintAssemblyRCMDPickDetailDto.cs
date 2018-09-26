using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrintAssemblyRCMDPickDetailDto
    {
        public Guid SysId { get; set; }

        public string AssemblyOrder { get; set; }
        public string Channel { get; set; }

        public List<PrintAssemblyPickDetailDto> PrintPickDetailDtos { get; set; }
    }
}
