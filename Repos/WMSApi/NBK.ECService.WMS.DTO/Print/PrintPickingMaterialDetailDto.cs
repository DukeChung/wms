using System;

namespace NBK.ECService.WMS.DTO
{
    public class PrintPickingMaterialDetailDto
    {
        public Guid ReceiptSysId { get; set; }

        public string ReceiptOrder { get; set; }

        public Guid SkuSysId { get; set; }

        public string SkuCode { get; set; }

        public string UPC { get; set; }

        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }

        public DateTime PickingDate { get; set; }

        public string PickingUserName { get; set; }
    }
}
