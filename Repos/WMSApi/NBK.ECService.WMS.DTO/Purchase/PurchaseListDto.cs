using System;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;

namespace NBK.ECService.WMS.DTO
{
    public class PurchaseListDto
    {
        public Guid? SysId { get; set; }
        public string PurchaseOrder { get; set; }
        public string ExternalOrder { get; set; }
        public string VendorName { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? AuditingDate { get; set; }
        public string AuditingName { get; set; }
        public int? Type { get; set; }
        public int? Status { get; set; }

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
                return ((PurchaseType)Type.Value).ToDescription();
            }
        }

        public string StatusText
        {
            get
            {
                if (Status.HasValue)
                {
                    return ConverStatus.Purchase(Status.Value);
                }
                else
                {
                    return string.Empty;
                }

            }
        }

        public string BatchNumber { get; set; }

        public string BusinessType { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }
    }
}