using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class receiptsnMap : EntityTypeConfiguration<receiptsn>
    {
        public receiptsnMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.SN)
                .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("receiptsn", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.ReceiptSysId).HasColumnName("ReceiptSysId");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.SN).HasColumnName("SN");

            // Relationships
            this.HasOptional(t => t.receipt)
                .WithMany(t => t.receiptsns)
                .HasForeignKey(d => d.ReceiptSysId);

        }
    }
}
