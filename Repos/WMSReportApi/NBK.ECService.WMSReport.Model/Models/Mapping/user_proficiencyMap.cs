using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class user_proficiencyMap : EntityTypeConfiguration<user_proficiency>
    {
        public user_proficiencyMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.UserName)
                .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("user_proficiency", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.Receipt).HasColumnName("Receipt");
            this.Property(t => t.Shelves).HasColumnName("Shelves");
            this.Property(t => t.Outbound).HasColumnName("Outbound");
            this.Property(t => t.PickDetail).HasColumnName("PickDetail");
            this.Property(t => t.Vanning).HasColumnName("Vanning");
            this.Property(t => t.Deliver).HasColumnName("Deliver");
        }
    }
}
