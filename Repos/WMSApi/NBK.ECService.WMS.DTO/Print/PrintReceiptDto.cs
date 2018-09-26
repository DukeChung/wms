using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PrintReceiptDto
    {
        public Guid? SysId { get; set; }
        public string ReceiptOrder { get; set; }

        public string ExternalOrder { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public string PurchaseDateDisplay
        {
            get
            {
                return PurchaseDate.HasValue ? PurchaseDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty;
            }
        }

        public DateTime? ReceipDate { get; set; }

        public string ReceipDateDisplay
        {
            get
            {
                return ReceipDate.HasValue ? ReceipDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty;
            }
        }

        public string VendorName { get; set; }

        public string VendorPhone { get; set; }

        public string VendorContacts { get; set; }

        public string PurchaseDescr { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int ReceiptType { get; set; }

        /// <summary>
        /// 来源仓
        /// </summary>
        public string FromWareHouseName { get; set; }
        /// <summary>
        /// 目标仓
        /// </summary>
        public string ToWareHouseName { get; set; }
        /// <summary>
        /// 移仓单号
        /// </summary>
        public string TransferInventoryOrder { get; set; }

        /// <summary>
        /// 作业人
        /// </summary>
        public string AppointUserNames { get; set; }

        public List<PrintReceiptDetailDto> PrintReceiptDetailList { get; set; }
    }
}
