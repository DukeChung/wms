using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class uomMap : EntityTypeConfiguration<uom>
    {
        public uomMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Table & Column Mappings
            this.ToTable("uom", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.UOMCode).HasColumnName("UOMCode");
            this.Property(t => t.Descr).HasColumnName("Descr");
            this.Property(t => t.UomType).HasColumnName("UomType");
        }
    }
}
