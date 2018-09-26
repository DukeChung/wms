using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class skuborrowMap : EntityTypeConfiguration<skuborrow>
    {
        public skuborrowMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            this.Property(t => t.CreateUserName)
                .IsRequired();

            this.Property(t => t.UpdateUserName)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("skuborrow", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.BorrowOrder).HasColumnName("BorrowOrder");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.AuditingBy).HasColumnName("AuditingBy");
            this.Property(t => t.AuditingName).HasColumnName("AuditingName");
            this.Property(t => t.AuditingDate).HasColumnName("AuditingDate");
            this.Property(t => t.BorrowStartTime).HasColumnName("BorrowStartTime");
            this.Property(t => t.BorrowEndTime).HasColumnName("BorrowEndTime");
            this.Property(t => t.IsDamage).HasColumnName("IsDamage");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.BorrowName).HasColumnName("BorrowName");
            this.Property(t => t.LendingDepartment).HasColumnName("LendingDepartment");
            this.Property(t => t.OtherId).HasColumnName("OtherId");
            this.Property(t => t.Channel).HasColumnName("Channel");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.TS).HasColumnName("TS").IsConcurrencyToken();
        }
    }
}
