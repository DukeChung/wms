using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class stockmovementMap : EntityTypeConfiguration<stockmovement>
    {
        public stockmovementMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.StockMovementOrder)
                .HasMaxLength(36);

            this.Property(t => t.Descr)
                .HasMaxLength(256);

            this.Property(t => t.Lot)
                .HasMaxLength(32);

            this.Property(t => t.Lpn)
                .HasMaxLength(32);

            this.Property(t => t.FromLoc)
                .HasMaxLength(32);

            this.Property(t => t.ToLoc)
                .HasMaxLength(32);

            this.Property(t => t.FromQty)
                .HasMaxLength(32);

            this.Property(t => t.ToQty)
                .HasMaxLength(32);

            this.Property(t => t.CreateUserName)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.UpdateUserName)
                .IsRequired()
                .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("stockmovement", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.StockMovementOrder).HasColumnName("StockMovementOrder");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Descr).HasColumnName("Descr");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.Lot).HasColumnName("Lot");
            this.Property(t => t.Lpn).HasColumnName("Lpn");
            this.Property(t => t.FromLoc).HasColumnName("FromLoc");
            this.Property(t => t.ToLoc).HasColumnName("ToLoc");
            this.Property(t => t.FromQty).HasColumnName("FromQty");
            this.Property(t => t.ToQty).HasColumnName("ToQty");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
        }
    }
}
