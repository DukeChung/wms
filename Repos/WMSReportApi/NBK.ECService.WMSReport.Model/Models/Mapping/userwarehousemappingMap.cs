using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class userwarehousemappingMap : EntityTypeConfiguration<userwarehousemapping>
    {
        public userwarehousemappingMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            this.Property(t => t.DisplayName)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.CreateUserName)
                .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("userwarehousemapping", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.DisplayName).HasColumnName("DisplayName");
            this.Property(t => t.WarehouseSysId).HasColumnName("WarehouseSysId");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
        }
    }
}
