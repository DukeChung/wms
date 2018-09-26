using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class ReceiptOperationUpdatePurchaseDto
    {
        public Guid PurchaseDetailSysId { get; set; }

        public string SkuUPC { get; set; }

        public int ReceivedQty { get; set; }

        public int RejectedQty { get; set; }

        public int GiftQty { get; set; }

        public int RejectedGiftQty { get; set; }
        /// <summary>
        /// 破损数量
        /// </summary>
        public int DamagedQuantity { get; set; }

        public ReceiptDetailOperationDto Rejected { get; set; }
    }
}
