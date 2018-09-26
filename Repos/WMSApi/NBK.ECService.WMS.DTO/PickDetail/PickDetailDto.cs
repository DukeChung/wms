using System;

namespace NBK.ECService.WMS.DTO
{
    public class PickDetailDto
    {
        public Guid? SysId { get; set;}
        public  Guid? WareHouseSysId { get; set; }
        public Guid? OutboundSysId { get; set; }
        public Guid? OutboundDetailSysId { get; set; }
        public string PickDetailOrder { get; set; }
        public DateTime? PickDate { get; set; }
        public int? Status { get; set; }
        public Guid? SkuSysId { get; set; }
        public Guid? UOMSysId { get; set; }
        public Guid? PackSysId { get; set; }
        public string Loc { get; set; }
        public string Lot { get; set; }
        public string Lpn { get; set; }
        public int? Qty { get; set; }
        public decimal DisplayQty { get; set; }
        public int PickedQty { get; set; }
        public decimal DisplayPickedQty { get; set; }
        public long CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}