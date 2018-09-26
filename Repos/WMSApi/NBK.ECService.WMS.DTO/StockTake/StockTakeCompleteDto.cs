using System;
using System.Collections.Generic;

namespace NBK.ECService.WMS.DTO
{
    public class StockTakeCompleteDto : BaseDto
    {
        public List<Guid> SysIds { get; set; }
    }
}
