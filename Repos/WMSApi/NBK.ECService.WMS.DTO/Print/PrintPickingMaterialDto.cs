using System;
using System.Collections.Generic;

namespace NBK.ECService.WMS.DTO
{
    public class PrintPickingMaterialDto
    {
        public Guid ReceiptSysId { get; set; }

        public string ReceiptOrder { get; set; }

        public DateTime PickingDate { get; set; }

        public string PickingUserName { get; set; }

        public List<PrintPickingMaterialDetailDto> PrintPickingMaterialDetailDtoList { get; set; }
    }
}
