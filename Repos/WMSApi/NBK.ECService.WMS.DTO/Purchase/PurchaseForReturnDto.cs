using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PurchaseForReturnDto
    {
        public Guid SysId { get; set; }

        public string PurchaseOrder { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string ExternalOrder { get; set; }
        public Guid VendorSysId { get; set; }
        public string Descr { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? AuditingDate { get; set; }
        public string AuditingBy { get; set; }
        public string AuditingName { get; set; }
        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public string Source { get; set; }
        public DateTime? LastReceiptDate { get; set; }
        public string PoGroup { get; set; }
        public DateTime? ClosedDate { get; set; }
        public Guid? WarehouseSysId { get; set; }
        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }
        public long? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }

        public string Channel { get; set; }
        public string BatchNumber { get; set; }

        public Guid? OutboundSysId { get; set; }
        public string OutboundOrder { get; set; }
        public string BusinessType { get; set; }
        public Guid? FromWareHouseSysId { get; set; }
        public List<PurchaseDetailForReturnDto> purchasedetails { get; set; }

        public PurchaseExtendForReturnDto PurchaseExtend { get; set; }
    }

    public class PurchaseDetailForReturnDto
    {
        public Guid SysId { get; set; }
        public Guid PurchaseSysId { get; set; }
        public Guid SkuSysId { get; set; }
        public Guid? SkuClassSysId { get; set; }
        public string UomCode { get; set; }
        public Guid UOMSysId { get; set; }
        public Guid? PackSysId { get; set; }
        public string PackCode { get; set; }
        public int Qty { get; set; }
        public int GiftQty { get; set; }
        public int ReceivedQty { get; set; }
        public int ReceivedGiftQty { get; set; }
        public int RejectedQty { get; set; }
        public int RejectedGiftQty { get; set; }
        public Nullable<decimal> LastPrice { get; set; }
        public Nullable<decimal> HistoryPrice { get; set; }
        public Nullable<decimal> PurchasePrice { get; set; }
        public string Remark { get; set; }
        public string OtherSkuId { get; set; }
        public string PackFactor { get; set; }

        public DateTime UpdateDate { get; set; }

        public long UpdateBy { get; set; }

        public string UpdateUserName { get; set; }
    }

    public class PurchaseExtendForReturnDto
    {
        public string ServiceStationName { get; set; }

        public string ServiceStationCode { get; set; }
    }
}
