using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.DTO.Base;

namespace NBK.ECService.WMSReport.DTO.Query
{
    public class ReceiptQuery : BaseQuery
    {
        public string ReceiptOrderSearch { get; set; }

        public string VendorNameSearch { get; set; }

        public int? StatusSearch { get; set; }

        public string ExternalOrderSearch { get; set; }

        /// <summary>
        /// 待上架收货单查询条件
        /// </summary>
        public bool WaitShelvesSearch { get; set; } = false;

        public DateTime? ReceiptDateFromSearch { get; set; }

        public DateTime? ReceiptDateToSearch { get; set; }

        public DateTime? CreateDateFromSearch { get; set; }

        public DateTime? CreateDateToSearch { get; set; }

        public bool? IsMaterial { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuNameSearch { get; set; }

        /// <summary>
        /// UPC
        /// </summary>
        public string UPCSearch { get; set; }

        /// <summary>
        /// 上架状态
        /// </summary>
        public int? ShelvesStatusSearch { get; set; }
    }
}
