using System;

namespace NBK.ECService.WMS.DTO
{
    public class VanningDetailDto
    {
        public Guid? SysId { get; set; }

        public Guid? VanningSysId { get; set; }
        public Guid? ContainerSysId { get; set; }
        public string ContainerNumber { get; set; }
        public Guid? CarrierSysId { get; set; }
        public string CarrierNumber { get; set; }
        public decimal? Weight { get; set; }
        public long CreateBy { get; set; }

        public string CreateUserName { get; set; }
        public DateTime? CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public string UpdateUserName { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? Status { get; set; }
        public string ExternOrderId { get; set; }

        public string ConsigneePhone { get; set; }
        public int? OutboundType { get; set; }
        public int VannginSkuCount { get; set; }

        public string Marke { get; set; }
    }
}