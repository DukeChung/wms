using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class UpdatePickDetailDto : BaseDto
    {
        public Guid SysId { get; set; }
        public Guid SkuSysId { get; set; }
        public int PickedQty { get; set; }
        public string Loc { get; set; }
        public string Lot { get; set; }
        public List<ContainerInfo> ContainerInfos { get; set; }
    }
}
