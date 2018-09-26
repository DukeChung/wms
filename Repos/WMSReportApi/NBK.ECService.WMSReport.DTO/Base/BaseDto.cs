using System;

namespace NBK.ECService.WMSReport.DTO.Base
{

    public class BaseDto
    {
        public int CurrentUserId { get; set; }
        public string CurrentDisplayName { get; set; }
        public Guid WarehouseSysId { get; set; }
    }
}