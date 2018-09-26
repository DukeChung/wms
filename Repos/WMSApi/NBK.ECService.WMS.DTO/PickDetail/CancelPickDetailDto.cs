using System;
using System.Collections.Generic;

namespace NBK.ECService.WMS.DTO
{
    public class CancelPickDetailDto : BaseDto
    {
        public List<string> PickDetailOrderList { get; set; }
    }
}
