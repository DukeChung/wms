using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class outboundMap : EntityTypeConfiguration<outbound>
    {
        public outboundMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Table & Column Mappings
            this.ToTable("outbound", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.OutboundOrder).HasColumnName("OutboundOrder");
            this.Property(t => t.OutboundDate).HasColumnName("OutboundDate");
            this.Property(t => t.RequestedShipDate).HasColumnName("RequestedShipDate");
            this.Property(t => t.ActualShipDate).HasColumnName("ActualShipDate");
            this.Property(t => t.DeliveryDate).HasColumnName("DeliveryDate");
            this.Property(t => t.Priority).HasColumnName("Priority");
            this.Property(t => t.OutboundType).HasColumnName("OutboundType");
            this.Property(t => t.OutboundGroup).HasColumnName("OutboundGroup");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.AuditingBy).HasColumnName("AuditingBy");
            this.Property(t => t.AuditingName).HasColumnName("AuditingName");
            this.Property(t => t.AuditingDate).HasColumnName("AuditingDate");
            this.Property(t => t.ExternOrderDate).HasColumnName("ExternOrderDate");
            this.Property(t => t.ExternOrderId).HasColumnName("ExternOrderId");
            this.Property(t => t.ReceiptSysId).HasColumnName("ReceiptSysId");
            this.Property(t => t.ConsigneeName).HasColumnName("ConsigneeName");
            this.Property(t => t.ConsigneeAddress).HasColumnName("ConsigneeAddress");
            this.Property(t => t.ConsigneeProvince).HasColumnName("ConsigneeProvince");
            this.Property(t => t.ConsigneeCity).HasColumnName("ConsigneeCity");
            this.Property(t => t.ConsigneeArea).HasColumnName("ConsigneeArea");
            this.Property(t => t.ConsigneePhone).HasColumnName("ConsigneePhone");
            this.Property(t => t.CashOnDelivery).HasColumnName("CashOnDelivery");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.TotalShippedQty).HasColumnName("TotalShippedQty");
            this.Property(t => t.TotalPickedQty).HasColumnName("TotalPickedQty");
            this.Property(t => t.TotalAllocatedQty).HasColumnName("TotalAllocatedQty");
            this.Property(t => t.TotalQty).HasColumnName("TotalQty");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.PostalCode).HasColumnName("PostalCode");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.Source).HasColumnName("Source");
            this.Property(t => t.Lng).HasColumnName("Lng");
            this.Property(t => t.Lat).HasColumnName("Lat");
            this.Property(t => t.ServiceStationName).HasColumnName("ServiceStationName");
            this.Property(t => t.TS).HasColumnName("TS").IsConcurrencyToken();
            this.Property(t => t.Channel).HasColumnName("Channel");
            this.Property(t => t.BatchNumber).HasColumnName("BatchNumber");
            this.Property(t => t.OutboundChildType).HasColumnName("OutboundChildType");
            this.Property(t => t.PlatformOrder).HasColumnName("PlatformOrder");
            this.Property(t => t.DiscountPrice).HasColumnName("DiscountPrice");
            this.Property(t => t.OutboundMethod).HasColumnName("OutboundMethod");
            this.Property(t => t.PurchaseOrder).HasColumnName("PurchaseOrder");
            this.Property(t => t.IsReturn).HasColumnName("IsReturn");
            this.Property(t => t.SortNumber).HasColumnName("SortNumber");
            this.Property(t => t.TMSOrder).HasColumnName("TMSOrder");
            this.Property(t => t.DepartureDate).HasColumnName("DepartureDate");
            this.Property(t => t.AppointUserNames).HasColumnName("AppointUserNames");
            this.Property(t => t.ServiceStationCode).HasColumnName("ServiceStationCode");
            this.Property(t => t.Exception).HasColumnName("Exception");
            this.Property(t => t.IsInvoice).HasColumnName("IsInvoice");
            this.Property(t => t.CouponPrice).HasColumnName("CouponPrice");

        }
    }
}
