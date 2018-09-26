using System;

namespace NBK.ECService.WMS.DTO
{
    public class VanningPickDetailDto
    {
        public string ContainerNumber { get; set; }
        public Guid? OutboundSysId { get; set; }
        public Guid? PickDetailSysId { get; set; }  
        public Guid? SkuSysId { get; set; }
        public string SkuCode { get; set; }
        public Guid? UOMSysId { get; set; }
        public Guid? PackSysId { get; set; }
        public string Loc { get; set; }
        public string Lot { get; set; }
        public string Lpn { get; set; }
        public int? Qty { get; set; }
        public long CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }

        public string CreateUserName { get; set; }

        public string UpdateUserName { get; set; }
    }
}