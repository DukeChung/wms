using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class syscodeMap : EntityTypeConfiguration<syscode>
    {
        public syscodeMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.SysCodeType)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.Descr)
                .HasMaxLength(64);

            // Table & Column Mappings
            this.ToTable("syscode", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.SysCodeType).HasColumnName("SysCodeType");
            this.Property(t => t.Descr).HasColumnName("Descr");
        }
    }
}
