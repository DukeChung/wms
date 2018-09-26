using System;

namespace NBK.ECService.WMS.DTO
{
    public class PickingMaterialQuery : BaseQuery
    {
        public Guid ReceiptSysId { get; set; }

        public string PickingUserName { get; set; }

        public int? PickingNumber { get; set; }
    }
}
