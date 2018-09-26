using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class locationMap : EntityTypeConfiguration<location>
    {
        public locationMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.Loc)
                .HasMaxLength(32);

            this.Property(t => t.LocUsage)
                .HasMaxLength(32);

            this.Property(t => t.LocCategory)
                .HasMaxLength(32);

            this.Property(t => t.LocFlag)
                .HasMaxLength(32);

            this.Property(t => t.LocHandling)
                .HasMaxLength(32);

            this.Property(t => t.UpdateUserName)
       .HasMaxLength(32);

            this.Property(t => t.CreateUserName)
            .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("location", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.Loc).HasColumnName("Loc");
            this.Property(t => t.LocUsage).HasColumnName("LocUsage");
            this.Property(t => t.LocCategory).HasColumnName("LocCategory");
            this.Property(t => t.LocFlag).HasColumnName("LocFlag");
            this.Property(t => t.LocHandling).HasColumnName("LocHandling");
            this.Property(t => t.ZoneSysId).HasColumnName("ZoneSysId");
            this.Property(t => t.LogicalLoc).HasColumnName("LogicalLoc");
            this.Property(t => t.XCoord).HasColumnName("XCoord");
            this.Property(t => t.YCoord).HasColumnName("YCoord");
            this.Property(t => t.LocLevel).HasColumnName("LocLevel");
            this.Property(t => t.Cube).HasColumnName("Cube");
            this.Property(t => t.Length).HasColumnName("Length");
            this.Property(t => t.Width).HasColumnName("Width");
            this.Property(t => t.Height).HasColumnName("Height");
            this.Property(t => t.CubicCapacity).HasColumnName("CubicCapacity");
            this.Property(t => t.WeightCapacity).HasColumnName("WeightCapacity");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.WarehouseSysId).HasColumnName("WarehouseSysId");

            // Relationships
            this.HasOptional(t => t.zone)
                .WithMany(t => t.locations)
                .HasForeignKey(d => d.ZoneSysId);

        }
    }
}
