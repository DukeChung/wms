using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class AddAssemblyDto : BaseDto
    {
        public Guid SkuSysId { get; set; }

        public DateTime PlanProcessingDate { get; set; }

        public DateTime PlanCompletionDate { get; set; }

        public List<AddAssemblyDetailDto> AddAssemblyDetailList { get; set; }
    }

    public class AddAssemblyDetailDto
    {
        public Guid SkuSysId { get; set; }

        public int UnitQty { get; set; }
    }
}
