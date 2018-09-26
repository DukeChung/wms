using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrintPurchaseViewDto
    {
        public string PurchaseOrder { get; set; }

        public string VendorName { get; set; }

        public string VendorContacts { get; set; }

        public string VendorPhone { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public string PurchaseDateText { get { return PurchaseDate.HasValue ? PurchaseDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public int? Status { get; set; }

        public string StatusText { get { return Status.HasValue ? ((Utility.Enum.PurchaseStatus)Status.Value).ToDescription() : string.Empty; } }

        public DateTime? DeliveryDate { get; set; }

        public string DeliveryDateText { get { return DeliveryDate.HasValue ? DeliveryDate.Value.ToString(PublicConst.DateFormat) : PublicConst.NotLimit; } }

        public DateTime? LastReceiptDate { get; set; }

        public string LastReceiptDateText { get { return LastReceiptDate.HasValue ? LastReceiptDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public string Descr { get; set; }
        /// <summary>
        /// 来源仓
        /// </summary>
        public string FromWareHouseName { get; set; }
        public int Type { get; set; }
        /// <summary>
        /// 目标仓
        /// </summary>
        public string ToWareHouseName { get; set; }
        /// <summary>
        /// 移仓单号
        /// </summary>
        public string TransferInventoryOrder { get; set; }

        public List<PrintPurchaseDetailViewDto> PrintPurchaseDetailViewDtoList { get; set; }
    }
}
