using System;

namespace NBK.ECService.WMS.DTO
{
    public class BaseDto
    {
        public int CurrentUserId { get; set; }
        public string CurrentDisplayName { get; set; }
        public Guid WarehouseSysId { get; set; }
    }
}