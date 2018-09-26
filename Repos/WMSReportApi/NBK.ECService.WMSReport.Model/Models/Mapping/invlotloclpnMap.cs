using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class invlotloclpnMap : EntityTypeConfiguration<invlotloclpn>
    {
        public invlotloclpnMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.Loc)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.Lot)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.Lpn)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.UpdateUserName)
                   .HasMaxLength(32);

            this.Property(t => t.CreateUserName)
            .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("invlotloclpn", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.Loc).HasColumnName("Loc");
            this.Property(t => t.Lot).HasColumnName("Lot");
            this.Property(t => t.Lpn).HasColumnName("Lpn");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.AllocatedQty).HasColumnName("AllocatedQty");
            this.Property(t => t.PickedQty).HasColumnName("PickedQty");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.FrozenQty).HasColumnName("FrozenQty");
        }
    }
}
