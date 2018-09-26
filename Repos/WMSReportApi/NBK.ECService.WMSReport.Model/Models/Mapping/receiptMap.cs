using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class receiptMap : EntityTypeConfiguration<receipt>
    {
        public receiptMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.ReceiptOrder)
                .IsRequired()
                .HasMaxLength(32);

            //this.Property(t => t.DisplayExternalOrder)
            //    .HasMaxLength(32);

            this.Property(t => t.ExternalOrder)
                .HasMaxLength(32);

            this.Property(t => t.Descr)
                .HasMaxLength(256);

            this.Property(t => t.ReturnDescr)
                .HasMaxLength(256);

            this.Property(t => t.UpdateUserName)
                   .HasMaxLength(32);

            this.Property(t => t.CreateUserName)
            .HasMaxLength(32);

            this.Property(t => t.AppointUserNames)
            .HasMaxLength(256);

            // Table & Column Mappings
            this.ToTable("receipt", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.ReceiptOrder).HasColumnName("ReceiptOrder");
            //this.Property(t => t.DisplayExternalOrder).HasColumnName("DisplayExternalOrder");
            this.Property(t => t.ExternalOrder).HasColumnName("ExternalOrder");
            this.Property(t => t.ReceiptType).HasColumnName("ReceiptType");
            this.Property(t => t.WarehouseSysId).HasColumnName("WarehouseSysId");
            this.Property(t => t.ExpectedReceiptDate).HasColumnName("ExpectedReceiptDate");
            this.Property(t => t.ReceiptDate).HasColumnName("ReceiptDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Descr).HasColumnName("Descr");
            this.Property(t => t.ReturnDescr).HasColumnName("ReturnDescr");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.VendorSysId).HasColumnName("VendorSysId");
            this.Property(t => t.ClosedDate).HasColumnName("ClosedDate");
            this.Property(t => t.ArrivalDate).HasColumnName("ArrivalDate");
            this.Property(t => t.TotalExpectedQty).HasColumnName("TotalExpectedQty");
            this.Property(t => t.TotalReceivedQty).HasColumnName("TotalReceivedQty");
            this.Property(t => t.TotalRejectedQty).HasColumnName("TotalRejectedQty");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.AppointUserNames).HasColumnName("AppointUserNames");
            this.Property(t => t.TS).HasColumnName("TS").IsConcurrencyToken();

            //// Relationships
            //this.HasRequired(t => t.warehouse)
            //    .WithMany(t => t.receipts)
            //    .HasForeignKey(d => d.WarehouseSysId);

        }
    }
}
