using System;
using System.Collections.Generic;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.DTO
{
    public class PrintAutoShelvesDto
    {
        public string ReceiptOrder { get; set; }

        public DateTime? ReceiptDate { get; set; }

        public string ReceipDateDisplay
        {
            get
            {
                return ReceiptDate.HasValue ? ReceiptDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty;
            }
        }

        public List<PrintReceiptDetailDto> PrintReceiptDetailDtos { get; set; }
    }
}