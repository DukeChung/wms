using System;

namespace NBK.ECService.WMS.DTO
{
    public class LotTemplateValueDto
    {
        public Guid SkuSysId { get; set; }

        public decimal Qty { get; set; }
        public int ReceivedQty { get; set; }
        public string LotValue01 { get; set; }
        public string LotValue02 { get; set; }
        public string LotValue03 { get; set; }
        public string LotValue04 { get; set; }
        public string LotValue05 { get; set; }
        public string LotValue06 { get; set; }
        public string LotValue07 { get; set; }
        public string LotValue08 { get; set; }
        public string LotValue09 { get; set; }
        public DateTime? ProduceDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string ExternalLot { get; set; }
    }
}