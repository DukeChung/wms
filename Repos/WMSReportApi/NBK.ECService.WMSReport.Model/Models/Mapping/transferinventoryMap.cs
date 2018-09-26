using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class transferinventoryMap : EntityTypeConfiguration<transferinventory>
    {
        public transferinventoryMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.TransferInventoryOrder)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.FromWareHouseName)
                .HasMaxLength(16);

            this.Property(t => t.ToWareHouseName)
                .IsRequired()
                .HasMaxLength(16);

            this.Property(t => t.TransferOutboundOrder)
                .HasMaxLength(32);

            this.Property(t => t.TransferPurchaseOrder)
                .HasMaxLength(32);

            this.Property(t => t.ExternOrderId)
                .HasMaxLength(32);

            this.Property(t => t.Remark)
                .HasMaxLength(128);

            this.Property(t => t.AuditingBy)
                .HasMaxLength(32);

            this.Property(t => t.AuditingName)
                .HasMaxLength(32);

            this.Property(t => t.ShippingMethod)
                .HasMaxLength(64);
            this.Property(t => t.UpdateUserName)
          .HasMaxLength(32);

            this.Property(t => t.CreateUserName)
            .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("transferinventory", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.TransferInventoryOrder).HasColumnName("TransferInventoryOrder");
            this.Property(t => t.FromWareHouseSysId).HasColumnName("FromWareHouseSysId");
            this.Property(t => t.FromWareHouseName).HasColumnName("FromWareHouseName");
            this.Property(t => t.ToWareHouseSysId).HasColumnName("ToWareHouseSysId");
            this.Property(t => t.ToWareHouseName).HasColumnName("ToWareHouseName");
            this.Property(t => t.TransferOutboundDate).HasColumnName("TransferOutboundDate");
            this.Property(t => t.TransferInboundDate).HasColumnName("TransferInboundDate");
            this.Property(t => t.TransferOutboundOrder).HasColumnName("TransferOutboundOrder");
            this.Property(t => t.TransferOutboundSysId).HasColumnName("TransferOutboundSysId");
            this.Property(t => t.TransferPurchaseOrder).HasColumnName("TransferPurchaseOrder");
            this.Property(t => t.TransferPurchaseSysId).HasColumnName("TransferPurchaseSysId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.ExternOrderDate).HasColumnName("ExternOrderDate");
            this.Property(t => t.ExternOrderId).HasColumnName("ExternOrderId");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.AuditingBy).HasColumnName("AuditingBy");
            this.Property(t => t.AuditingName).HasColumnName("AuditingName");
            this.Property(t => t.AuditingDate).HasColumnName("AuditingDate");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.ShippingMethod).HasColumnName("ShippingMethod");
            this.Property(t => t.Freight).HasColumnName("Freight");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
        }
    }
}
