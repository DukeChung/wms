using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class stockmovementMap : EntityTypeConfiguration<stockmovement>
    {
        public stockmovementMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            this.Property(t => t.CreateUserName)
                .IsRequired();

            this.Property(t => t.UpdateUserName)
                .IsRequired();

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
            this.Property(t => t.TS).HasColumnName("TS").IsConcurrencyToken();
        }
    }
}
