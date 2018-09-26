using System;
using System.Collections.Generic;

namespace NBK.ECService.WMS.DTO
{
    public class ReceiptCollectionLotDto : BaseDto
    {
        public Guid ReceiptSysId { get; set; }

        public Guid? SkuSysId { get; set; }

        public string UPC { get; set; }

        public List<LotTemplateValueDto> LotTemplateValueDtos { get; set; }
    }
}
