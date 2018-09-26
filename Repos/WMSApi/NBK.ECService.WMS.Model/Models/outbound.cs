using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class outbound : SysIdEntity
    {
        public outbound()
        {
            this.outbounddetails = new List<outbounddetail>();
        }

        public System.Guid WareHouseSysId { get; set; }
        public string OutboundOrder { get; set; }
        public Nullable<System.DateTime> OutboundDate { get; set; }
        public Nullable<System.DateTime> RequestedShipDate { get; set; }
        public Nullable<System.DateTime> ActualShipDate { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public Nullable<int> Priority { get; set; }
        public Nullable<int> OutboundType { get; set; }
        public string OutboundGroup { get; set; }
        public Nullable<int> Status { get; set; }
        public string AuditingBy { get; set; }
        public string AuditingName { get; set; }
        public Nullable<System.DateTime> AuditingDate { get; set; }
        public Nullable<System.DateTime> ExternOrderDate { get; set; }
        public string ExternOrderId { get; set; }
        public Nullable<System.Guid> ReceiptSysId { get; set; }
        public string ConsigneeName { get; set; }
        public string ConsigneeAddress { get; set; }
        public string ConsigneeProvince { get; set; }
        public string ConsigneeCity { get; set; }
        public string ConsigneeArea { get; set; }
        public string ConsigneePhone { get; set; }
        public string PostalCode { get; set; }
        public Nullable<int> CashOnDelivery { get; set; }
        public string Remark { get; set; }
        public Nullable<int> TotalShippedQty { get; set; }
        public Nullable<int> TotalPickedQty { get; set; }
        public Nullable<int> TotalAllocatedQty { get; set; }
        public Nullable<int> TotalQty { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }

        public string ConsigneeTown { get; set; }

        public string ConsigneeVillage { get; set; }

        public string ShippingMethod { get; set; }

        public string Source { get; set; }

        public string ServiceStationName { get; set; }

        public Nullable<int> InvoiceType { get; set; }

        public Nullable<decimal> Freight { get; set; }

        public Nullable<decimal> TotalPrice { get; set; }

        public string CreateUserName { get; set; }
        public string UpdateUserName { get; set; }

        public System.Guid TS { get; set; }

        public string Lng { get; set; }
        public string Lat { get; set; }
        public string Channel { get; set; }
        public string BatchNumber { get; set; }
        public string OutboundChildType { get; set; }
        public string PlatformOrder { get; set; }
        public Nullable<decimal> DiscountPrice { get; set; }
        public string OutboundMethod { get; set; }
        public string PurchaseOrder { get; set; }
        public Nullable<int> IsReturn { get; set; }

        public Nullable<int> SortNumber { get; set; }
        public string TMSOrder { get; set; }
        public Nullable<System.DateTime> DepartureDate { get; set; }
        public string AppointUserNames { get; set; }
        public string ServiceStationCode { get; set; }
        public Nullable<bool> Exception { get; set; }
        /// <summary>
        /// 是否开票 默认false
        /// </summary>
        public bool IsInvoice { get; set; }
        /// <summary>
        /// 优惠券价格 默认0
        /// </summary>
        public decimal CouponPrice { get; set; }
        public virtual ICollection<outbounddetail> outbounddetails { get; set; }
    }
}
