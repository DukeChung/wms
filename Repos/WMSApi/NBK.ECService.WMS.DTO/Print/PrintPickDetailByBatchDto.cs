using System.Collections.Generic;

namespace NBK.ECService.WMS.DTO
{
    public class PrintPickDetailByBatchDto
    {
        public string PickDetailOrder { get; set; }

        public int OrderCount { get; set; }

        public int SkuCount { get; set; }
        public int SkuQty { get; set; }

        public List<PrintPickDetailDto> PrintPickDetailDtos { get; set; }
    }
}