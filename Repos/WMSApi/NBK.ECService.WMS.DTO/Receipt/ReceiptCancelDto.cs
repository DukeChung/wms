using System;

namespace NBK.ECService.WMS.DTO
{
    public class ReceiptCancelDto : BaseDto
    {
        /// <summary>
        /// 入库单Id
        /// </summary>
        public Guid PurchaseSysId { get; set; }

        /// <summary>
        /// 入库单号
        /// </summary>
        public string PurchaseOrder { get; set; }
    }
}
