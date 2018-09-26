using System;

namespace NBK.ECService.WMS.DTO
{
    public class VanningDeliveryDto
    {
        public Guid? InvLotLocLpnSysId { get; set; }
        public Guid? InvLotSysId { get; set; }
        public Guid? InvSkuLocSysId { get; set; }
     
        public Guid? OutboundSysId { get; set; }
        public Guid? OutboundDetailSysId { get; set; }
        public Guid? PickDetailSysId { get; set; }
        public Guid? SkuSysId { get; set; }
        public string Loc { get; set; }
        public string Lot { get; set; }
        public string Lpn { get; set; }
        public int? Qty { get; set; }
    }
}