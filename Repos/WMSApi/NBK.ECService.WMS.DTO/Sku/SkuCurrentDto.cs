using System;

namespace NBK.ECService.WMS.DTO
{
    public class SkuCurrentDto
    {
        public Guid SysId { get; set; }
        public string SkuCode { get; set; }
        public string SkuName { get; set; }
        public string SkuDescr { get; set; }
        public string SkuUPC { get; set; }
        public string SkuImage { get; set; }
    }
}