using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFContainerPickingDto
    {
        public List<RFContainerPickingDetailListDto> PickingDetails { get; set; }

        public List<RFContainerPickingDetailListDto> GroupedPickingDetails { get; set; }
    }
}
