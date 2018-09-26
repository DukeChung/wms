using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class vendor_distribution_detailMap : EntityTypeConfiguration<vendor_distribution_detail>
    {
        public vendor_distribution_detailMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.PurcharOrder)
                .HasMaxLength(32);

            this.Property(t => t.CreateByName)
                .HasMaxLength(32);

            this.Property(t => t.WareHouseName)
                .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("vendor_distribution_detail", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.VendorDistributionSysId).HasColumnName("VendorDistributionSysId");
            this.Property(t => t.PurcharSysId).HasColumnName("PurcharSysId");
            this.Property(t => t.PurcharOrder).HasColumnName("PurcharOrder");
            this.Property(t => t.SkuQty).HasColumnName("SkuQty");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateByName).HasColumnName("CreateByName");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.WareHouseName).HasColumnName("WareHouseName");
            this.Property(t => t.ProcurementTime).HasColumnName("ProcurementTime");

            // Relationships
            this.HasOptional(t => t.vendor_distribution)
                .WithMany(t => t.vendor_distribution_detail)
                .HasForeignKey(d => d.VendorDistributionSysId);

        }
    }
}
