using System;

namespace NBK.ECService.WMS.DTO
{
    public class LotTemplateListDto
    { 
        public Guid SysId { get; set; }
        public string LotCode { get; set; }
        public string Descr { get; set; }
    }
}