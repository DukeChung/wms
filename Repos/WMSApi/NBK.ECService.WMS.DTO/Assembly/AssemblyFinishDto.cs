using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class AssemblyFinishDto : BaseDto
    {
        public Guid SysId { get; set; }

        public int ActualQty { get; set; }

        public List<AssemblyFinishDetailDto> AssemblyDetails { get; set; }
    }

    public class AssemblyFinishDetailDto
    {
        public Guid SkuSysId { get; set; }

        public decimal LossQty { get; set; }
    }
}
