using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.Utility;

namespace NBK.ECService.WMSReport.DTO.Other
{
    public class PurchaseViewDto : PurchaseDto
    {
        public DateTime? LastReceiptDate { get; set; }

        public string PurchaseDateText
        {
            get
            {
                if (PurchaseDate.HasValue)
                {
                    return PurchaseDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return string.Empty; ;
                }
            }
        }

        public string AuditingDateText
        {
            get
            {
                if (AuditingDate.HasValue)
                {
                    return AuditingDate.Value.ToString(PublicConst.DateTimeFormat) + "[" + AuditingName + "]";
                }
                else
                {
                    return string.Empty; ;
                }
            }
        }
        public string LastReceiptDateText
        {
            get
            {
                if (LastReceiptDate != null)
                {
                    return LastReceiptDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return PublicConst.NotInbound;
                }
            }
        }

        public string TypeText
        {

            get
            {
                if (Type.HasValue)
                {
                    return "";
                }
                else
                {
                    return string.Empty;
                }

            }
        }

        public string DeliveryDateText
        {
            get
            {
                if (DeliveryDate != null)
                {
                    return DeliveryDate.Value.ToString(PublicConst.DateTimeFormat);
                }
                else
                {
                    return PublicConst.NotLimit;
                }
            }
        }
        public int? OutboundType { get; set; }
        public int? IsReturn { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }
        public new virtual List<PurchaseDetailViewDto> PurchaseDetailViewDto { get; set; }

        public virtual List<ReceiptPurchaseDto> ReceiptPurchaseDto { get; set; }
    }
}
