using System;

namespace NBK.ECService.WMS.DTO
{
    public class InvSkuLocDto
    {
        public Guid? SkuSysId { get; set; }

        public string SkuName { get; set; }
        public string SkuUpc { get; set; }

        public string Loc { get; set; }

        public int LocationStatus { get; set; }

        public int Qty { get; set; }
    }
}