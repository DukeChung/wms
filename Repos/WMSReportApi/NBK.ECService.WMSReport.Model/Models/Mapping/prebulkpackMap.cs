using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class prebulkpackMap : EntityTypeConfiguration<prebulkpack>
    {
        public prebulkpackMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.PreBulkPackOrder)
                .HasMaxLength(32);

            this.Property(t => t.StorageCase)
                .HasMaxLength(32);

            this.Property(t => t.CreateUserName)
                .HasMaxLength(32);

            this.Property(t => t.UpdateUserName)
                .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("prebulkpack", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.PreBulkPackOrder).HasColumnName("PreBulkPackOrder");
            this.Property(t => t.StorageCase).HasColumnName("StorageCase");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
        }
    }
}
