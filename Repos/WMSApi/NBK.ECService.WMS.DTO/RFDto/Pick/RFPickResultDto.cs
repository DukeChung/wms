using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFPickResultDto
    {
        /// <summary>
        /// 已拣
        /// </summary>
        public List<RFContainerPickingDetailListDto> PickedList { get; set; }

        /// <summary>
        /// 待拣
        /// </summary>
        public List<RFContainerPickingDetailListDto> ToPickList { get; set; }

        /// <summary>
        /// 拣货差异
        /// </summary>
        public List<RFContainerPickingDetailListDto> PickDiffList { get; set; }
    }
}
