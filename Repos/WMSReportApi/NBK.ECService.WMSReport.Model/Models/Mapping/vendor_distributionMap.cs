using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class vendor_distributionMap : EntityTypeConfiguration<vendor_distribution>
    {
        public vendor_distributionMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.VendorName)
                .HasMaxLength(32);

            this.Property(t => t.PurcharOrder)
                .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("vendor_distribution", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.VendorSysId).HasColumnName("VendorSysId");
            this.Property(t => t.VendorName).HasColumnName("VendorName");
            this.Property(t => t.PurcharPeriod).HasColumnName("PurcharPeriod");
            this.Property(t => t.LastOrderDate).HasColumnName("LastOrderDate");
            this.Property(t => t.LastDistributionDate).HasColumnName("LastDistributionDate");
            this.Property(t => t.LastDistributionSkuQty).HasColumnName("LastDistributionSkuQty");
            this.Property(t => t.LastDistributionQty).HasColumnName("LastDistributionQty");
            this.Property(t => t.NextDistributionDate).HasColumnName("NextDistributionDate");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.Longitude).HasColumnName("Longitude");
            this.Property(t => t.Latitude).HasColumnName("Latitude");
            this.Property(t => t.PurcharOrder).HasColumnName("PurcharOrder");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.TotalQty).HasColumnName("TotalQty");
        }
    }
}
