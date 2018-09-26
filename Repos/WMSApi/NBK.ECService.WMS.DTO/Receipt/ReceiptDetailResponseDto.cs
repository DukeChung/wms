using System;

namespace NBK.ECService.WMS.DTO
{
    public class ReceiptDetailResponseDto : CommonResponse
    {
        public string ToLot { get; set; }

        public string LotAttr01 { get; set; }
    }
}
