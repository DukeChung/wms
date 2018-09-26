using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class adjustmentdetailMap : EntityTypeConfiguration<adjustmentdetail>
    {
        public adjustmentdetailMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.AdjustlevelCode)
                .HasMaxLength(32);

            this.Property(t => t.Loc)
                .HasMaxLength(32);

            this.Property(t => t.Lot)
                .HasMaxLength(32);

            this.Property(t => t.Lpn)
                .HasMaxLength(32);

            this.Property(t => t.Remark)
                .HasMaxLength(128);

            this.Property(t => t.CreateUserName)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.UpdateUserName)
                .IsRequired()
                .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("adjustmentdetail", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.AdjustmentSysId).HasColumnName("AdjustmentSysId");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.AdjustlevelCode).HasColumnName("AdjustlevelCode");
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

            // Relationships
            this.HasOptional(t => t.adjustment)
                .WithMany(t => t.adjustmentdetails)
                .HasForeignKey(d => d.AdjustmentSysId);

        }
    }
}
