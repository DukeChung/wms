using System;

namespace NBK.ECService.WMS.DTO
{
    public class PreOrderRuleDto : BaseDto
    {
        public Guid? SysId { get; set; }
        public bool? Status { get; set; }
        public int? MatchingRate { get; set; }

        public int? MatchingMaxRate { get; set; }

        /// <summary>
        /// 服务站
        /// </summary>
        public bool? ServiceStation { get; set; }

        public bool? MatchingSku { get; set; }


        public bool? MatchingQty { get; set; }
        public bool? DeliveryIntercept { get; set; }
        public bool? ExceedQty { get; set; }
    }
}