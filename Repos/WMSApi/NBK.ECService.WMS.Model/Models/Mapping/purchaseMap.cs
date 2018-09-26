using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class purchaseMap : EntityTypeConfiguration<purchase>
    {
        public purchaseMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.PurchaseOrder)
                .IsRequired();

            this.Property(t => t.Source)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("purchase", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.PurchaseOrder).HasColumnName("PurchaseOrder");
            this.Property(t => t.DeliveryDate).HasColumnName("DeliveryDate");
            this.Property(t => t.ExternalOrder).HasColumnName("ExternalOrder");
            this.Property(t => t.VendorSysId).HasColumnName("VendorSysId");
            this.Property(t => t.Descr).HasColumnName("Descr");
            this.Property(t => t.PurchaseDate).HasColumnName("PurchaseDate");
            this.Property(t => t.AuditingDate).HasColumnName("AuditingDate");
            this.Property(t => t.AuditingBy).HasColumnName("AuditingBy");
            this.Property(t => t.AuditingName).HasColumnName("AuditingName");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.Source).HasColumnName("Source");
            this.Property(t => t.LastReceiptDate).HasColumnName("LastReceiptDate");
            this.Property(t => t.PoGroup).HasColumnName("PoGroup");
            this.Property(t => t.ClosedDate).HasColumnName("ClosedDate");
            this.Property(t => t.WarehouseSysId).HasColumnName("WarehouseSysId");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.Channel).HasColumnName("Channel");
            this.Property(t => t.BatchNumber).HasColumnName("BatchNumber");
            this.Property(t => t.OutboundSysId).HasColumnName("OutboundSysId");
            this.Property(t => t.OutboundOrder).HasColumnName("OutboundOrder");
            this.Property(t => t.BusinessType).HasColumnName("BusinessType");
            this.Property(t => t.FromWareHouseSysId).HasColumnName("FromWareHouseSysId");

        }
    }
}
