using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class SaveQualityControlDto : BaseDto
    {
        public Guid QualityControlSysId { get; set; }

        public List<QualityControlDetailDto> QCDetails { get; set; }
    }
}
