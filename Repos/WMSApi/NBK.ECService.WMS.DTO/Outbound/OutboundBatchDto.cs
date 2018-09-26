using System;
using System.Collections.Generic;

namespace NBK.ECService.WMS.DTO.Outbound
{
    public class OutboundBatchDto:BaseDto
    {
        public Guid SysId { get; set; }
        public string OutboundOrder { get; set; }
        public string OutboundGroup { get; set; } 
        public string ConsigneeName { get; set; }
        public string ConsigneeAddress { get; set; }
        public string ConsigneePhone { get; set; }
        public DateTime? OutboundDate { get; set; }

        public int? TotalQty { get; set; }

        public decimal? TotalPrice { get; set; }

        public List<OutboundDetailBatchDto> BatchOutboundDetailDtos { get; set; }
    }
}