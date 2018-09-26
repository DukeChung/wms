using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PurchaseDetailReportQuery : BaseQuery
    {
        /// <summary>
        /// 入库单号
        /// </summary>
        public string PurchaseOrder { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }
        /// <summary>
        /// UPC
        /// </summary>
        public string UPC { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }
        /// <summary>
        /// 最后入库开始日期
        /// </summary>

        public DateTime? LastReceiptDateFrom { get; set; }
        /// <summary>
        /// 最后入库结束日期
        /// </summary>

        public DateTime? LastReceiptDateTo { get; set; }
    }

    public class PurchaseDetailReportDto : BaseDto
    {
        /// <summary>
        /// 入库单号
        /// </summary>
        public string PurchaseOrder { get; set; }
        /// <summary>
        /// 收货单号
        /// </summary>
        public string ReceiptOrder { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }
        /// <summary>
        /// UPC
        /// </summary>
        public string UPC { get; set; }
        /// <summary>
        /// 采购数量
        /// </summary>
        public decimal Qty { get; set; }
        /// <summary>
        /// 实际入库数量
        /// </summary>
        public decimal ReceivedQty { get; set; }
        /// <summary>
        /// 绝收数量
        /// </summary>
        public decimal RejectedQty { get; set; }

        //public string UomCode { set; get; }
        //public string PackCode { set; get; }
        /// <summary>
        /// 最后入库日期
        /// </summary>
        public DateTime? LastReceiptDate { get; set; }
        public string LastReceiptDateDisplay { get { return LastReceiptDate.HasValue ? LastReceiptDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }
    }
}
