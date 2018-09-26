using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrintReceiptDetailDto
    {
        public Guid SkuSysId { get; set; }
        public string SkuCode { get; set; }
        public string SkuName { get; set; }
        public string SkuDescr { get; set; }
        public string UPC { get; set; }

        public string ToLoc { get; set; }

        public string Lot { get; set; }

        public string UOMCode { get; set; }

        public int PurchaseQty { get; set; }
        public int ReceivedQty { get; set; }

        public int ShelveQty { get; set; }

        public decimal DisplayPurchaseQty { get; set; }
        public decimal DisplayReceivedQty { get; set; }

        public decimal DisplayShelveQty { get; set; }

        public int? CurrentReceivedQty { get; set; }

        public decimal DisplayCurrentReceivedQty { get; set; }

        public int GiftQty { get; set; }

        /// <summary>
        /// 破损数量
        /// </summary>
        public int AdjustmentQty { get; set; }


        public string Remark { get; set; }

        public string PackFactor { get; set; }

        public string OtherId { get; set; }
    }
}
