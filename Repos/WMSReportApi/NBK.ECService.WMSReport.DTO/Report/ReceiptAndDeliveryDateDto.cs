using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class ReceiptAndDeliveryDateDto
    {
        /// <summary>
        /// 入库单号
        /// </summary>
        public string PurchaseOrder { get; set; }

        public int? Type { get; set; }
        public string TypeName
        {
            get
            {
                return ((PurchaseType)Type.Value).ToDescription();
            }
        }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditingDate { get; set; }

        public string AuditingDateDisplay
        {
            get
            {
                if (AuditingDate.HasValue)
                {
                    return AuditingDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        /// <summary>
        /// 收货单号
        /// </summary>
        public string ReceiptOrder { get; set; }
        /// <summary>
        /// 收货单创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }
        public string CreateDateDisplay
        {
            get
            {
                if (CreateDate.HasValue)
                {
                    return CreateDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        /// <summary>
        /// 实际收货数量
        /// </summary>
        public decimal TotalReceivedQty { get; set; }
        /// <summary>
        /// 收货完成时间
        /// </summary>
        public DateTime? ReceiptDate { get; set; }
        public string ReceiptDateDisplay
        {
            get
            {
                if (ReceiptDate.HasValue)
                {
                    return ReceiptDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        /// <summary>
        /// 实际上架数量
        /// </summary>
        public decimal TotalShelvesQty { get; set; }
        /// <summary>
        /// 上架完成时间
        /// </summary>
        public DateTime? ShelvesDate { get; set; }
        public string ShelvesDateDisplay
        {
            get
            {
                if (ShelvesDate.HasValue)
                {
                    return ShelvesDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
