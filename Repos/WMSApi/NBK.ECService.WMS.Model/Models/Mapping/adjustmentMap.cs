using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class adjustmentMap : EntityTypeConfiguration<adjustment>
    {
        public adjustmentMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            this.Property(t => t.CreateUserName)
                .IsRequired();

            this.Property(t => t.UpdateUserName)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("adjustment", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.AdjustmentOrder).HasColumnName("AdjustmentOrder");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.AuditingDate).HasColumnName("AuditingDate");
            this.Property(t => t.AuditingBy).HasColumnName("AuditingBy");
            this.Property(t => t.AuditingName).HasColumnName("AuditingName");
            this.Property(t => t.SourceType).HasColumnName("SourceType");
            this.Property(t => t.SourceOrder).HasColumnName("SourceOrder");
        }
    }
}
