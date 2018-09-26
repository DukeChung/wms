using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class skuclassMap : EntityTypeConfiguration<skuclass>
    {
        public skuclassMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.SkuClassName)
                .HasMaxLength(64);
            // Properties
            this.Property(t => t.OtherId)
                .HasMaxLength(64);
            // Properties
            this.Property(t => t.Source)
                .HasMaxLength(32);
            this.Property(t => t.UpdateUserName)
       .HasMaxLength(32);

            this.Property(t => t.CreateUserName)
            .HasMaxLength(32);
            // Table & Column Mappings
            this.ToTable("skuclass", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.SkuClassName).HasColumnName("SkuClassName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.ParentSysId).HasColumnName("ParentSysId");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.OtherId).HasColumnName("OtherId");
            this.Property(t => t.Source).HasColumnName("Source");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");





        }
    }
}
