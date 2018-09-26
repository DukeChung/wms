using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class sku_salesMap : EntityTypeConfiguration<sku_sales>
    {
        public sku_salesMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.SkuName)
                .HasMaxLength(32);

            this.Property(t => t.UPC)
                .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("sku_sales", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.SkuName).HasColumnName("SkuName");
            this.Property(t => t.UPC).HasColumnName("UPC");
            this.Property(t => t.IsAssemblyParts).HasColumnName("IsAssemblyParts");
            this.Property(t => t.ReceiptQty).HasColumnName("ReceiptQty");
            this.Property(t => t.OutboundQty).HasColumnName("OutboundQty");
            this.Property(t => t.MinPrice).HasColumnName("MinPrice");
            this.Property(t => t.WeightedMeanPrice).HasColumnName("WeightedMeanPrice");
            this.Property(t => t.MaxPrice).HasColumnName("MaxPrice");
            this.Property(t => t.AveragePrice).HasColumnName("AveragePrice");
        }
    }
}
