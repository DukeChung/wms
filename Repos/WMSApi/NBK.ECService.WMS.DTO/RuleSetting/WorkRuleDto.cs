using System;
namespace NBK.ECService.WMS.DTO
{
    public class WorkRuleDto : BaseDto
    {
        public Guid? SysId { get; set; }
        public bool? Status { get; set; }
        public bool? PickWork { get; set; }
        public bool? ShelvesWork { get; set; }
        public bool? ReceiptWork { get; set; }
    
     
    }
}