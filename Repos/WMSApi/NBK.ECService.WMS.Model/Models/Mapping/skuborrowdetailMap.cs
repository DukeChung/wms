using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class skuborrowdetailMap : EntityTypeConfiguration<skuborrowdetail>
    {
        public skuborrowdetailMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            this.Property(t => t.CreateUserName)
                .IsRequired();

            this.Property(t => t.UpdateUserName)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("skuborrowdetail", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.SkuBorrowSysId).HasColumnName("SkuBorrowSysId");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.Loc).HasColumnName("Loc");
            this.Property(t => t.Lot).HasColumnName("Lot");
            this.Property(t => t.Lpn).HasColumnName("Lpn");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.BorrowStartTime).HasColumnName("BorrowStartTime");
            this.Property(t => t.BorrowEndTime).HasColumnName("BorrowEndTime");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.IsDamage).HasColumnName("IsDamage");
            this.Property(t => t.DamageReason).HasColumnName("DamageReason");
            this.Property(t => t.ReturnQty).HasColumnName("ReturnQty");
            this.Property(t => t.TS).HasColumnName("TS").IsConcurrencyToken();

            // Relationships
            this.HasOptional(t => t.skuborrow)
                .WithMany(t => t.skuborrowdetails)
                .HasForeignKey(d => d.SkuBorrowSysId);

        }
    }
}
