using System;

namespace NBK.ECService.WMS.DTO
{
    public class ReceiptCollectionLotQuery : BaseQuery
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid? SkuSysId { get; set; }

        /// <summary>
        /// 商品UPC
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 收货单ID
        /// </summary>
        public Guid ReceiptSysId { get; set; }
    }
}
