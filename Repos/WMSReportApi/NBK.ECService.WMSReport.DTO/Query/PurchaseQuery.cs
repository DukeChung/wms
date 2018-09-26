using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.DTO.Base;

namespace NBK.ECService.WMSReport.DTO.Query
{
    public class PurchaseQuery : BaseQuery
    {
        public string PurchaseOrderSearch { get; set; }
        public string ExternalOrderSearch { get; set; }
        public string VendorNameSearch { get; set; }
        public int? StatusSearch { get; set; }
        public string UpcCodeSearch { get; set; }
        public string SkuCodeSearch { get; set; }
        public string SkuNameSearch { get; set; }
        public string ReceiptOrderSearch { get; set; }
        public string TransferInventoryOrderSearch { get; set; }
        public Guid? ToWareHouseSysId { get; set; }


        public DateTime? ReceiptStartDateSearch { get; set; }

        public DateTime? ReceiptEndDateSearch { get; set; }

        public int? TypeSearch { get; set; }


        public DateTime? AuditingDateFrom { get; set; }

        public DateTime? AuditingDateTo { get; set; }

        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundOrderSearch { get; set; }

        public string BatchNumber { get; set; }

        /// <summary>
        /// 是否为部分退货入库单据
        /// </summary>
        public bool IsPurchaseReturn { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }
    }
}
