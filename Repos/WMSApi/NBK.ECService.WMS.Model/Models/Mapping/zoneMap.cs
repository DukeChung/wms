using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class zoneMap : EntityTypeConfiguration<zone>
    {
        public zoneMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.ZoneCode)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("zone", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.ZoneCode).HasColumnName("ZoneCode");
            this.Property(t => t.DefaultPickToLoc).HasColumnName("DefaultPickToLoc");
            this.Property(t => t.InLoc).HasColumnName("InLoc");
            this.Property(t => t.OutLoc).HasColumnName("OutLoc");
            this.Property(t => t.Descr).HasColumnName("Descr");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.WarehouseSysId).HasColumnName("WarehouseSysId");
        }
    }
}
