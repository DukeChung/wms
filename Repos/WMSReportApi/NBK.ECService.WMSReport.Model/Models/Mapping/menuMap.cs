using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class menuMap : EntityTypeConfiguration<menu>
    {
        public menuMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.MenuName)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.Action)
                .IsRequired()
                .HasMaxLength(16);

            this.Property(t => t.Controller)
                .IsRequired()
                .HasMaxLength(16);

            this.Property(t => t.ICons)
                .HasMaxLength(32);

            this.Property(t => t.GroupMenuController)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("menu", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.MenuName).HasColumnName("MenuName");
            this.Property(t => t.Action).HasColumnName("Action");
            this.Property(t => t.Controller).HasColumnName("Controller");
            this.Property(t => t.ICons).HasColumnName("ICons");
            this.Property(t => t.ParentSysId).HasColumnName("ParentSysId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.SortSequence).HasColumnName("SortSequence");
            this.Property(t => t.GroupMenuController).HasColumnName("GroupMenuController");
        }
    }
}
