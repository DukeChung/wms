using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class uomMap : EntityTypeConfiguration<uom>
    {
        public uomMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.UOMCode)
                .HasMaxLength(32);

            this.Property(t => t.Descr)
                .HasMaxLength(128);

            this.Property(t => t.UomType)
                .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("uom", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.UOMCode).HasColumnName("UOMCode");
            this.Property(t => t.Descr).HasColumnName("Descr");
            this.Property(t => t.UomType).HasColumnName("UomType");
        }
    }
}
