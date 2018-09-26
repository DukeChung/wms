using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class vendor_sku_priceMap : EntityTypeConfiguration<vendor_sku_price>
    {
        public vendor_sku_priceMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.SkuName)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.SkuUPC)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.VendorName)
                .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("vendor_sku_price", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.SkuName).HasColumnName("SkuName");
            this.Property(t => t.SkuUPC).HasColumnName("SkuUPC");
            this.Property(t => t.VendorSysId).HasColumnName("VendorSysId");
            this.Property(t => t.VendorName).HasColumnName("VendorName");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.PurchaseSysId).HasColumnName("PurchaseSysId");
            this.Property(t => t.Qty).HasColumnName("Qty");
        }
    }
}
