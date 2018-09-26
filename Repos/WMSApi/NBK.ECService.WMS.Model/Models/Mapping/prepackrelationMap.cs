using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class prepackrelationMap : EntityTypeConfiguration<prepackrelation>
    {
        public prepackrelationMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);
            // Table & Column Mappings
            this.ToTable("prepackrelation", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.PreBulkPackSysId).HasColumnName("PreBulkPackSysId");
            this.Property(t => t.PreBulkPackDetailSysId).HasColumnName("PreBulkPackDetailSysId");
            this.Property(t => t.PrePackSysId).HasColumnName("PrePackSysId");
            this.Property(t => t.PrePackDetailSysId).HasColumnName("PrePackDetailSysId");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
        }
    }
}