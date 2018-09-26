using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PartShipmentMemoDto : BaseDto
    {
        public List<PartShipmentDetailDto> PartShipmentDetailMemoList { get; set; }
    }
}
