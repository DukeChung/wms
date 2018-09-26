using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class operationlogMap : EntityTypeConfiguration<operationlog>
    {
        public operationlogMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.ApiController)
                .IsRequired();

            this.Property(t => t.AppService)
                .IsRequired();

            this.Property(t => t.UserName)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("operationlog", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.ApiController).HasColumnName("ApiController");
            this.Property(t => t.AppService).HasColumnName("AppService");
            this.Property(t => t.Descr).HasColumnName("Descr");
            this.Property(t => t.JsonValue).HasColumnName("JsonValue");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
        }
    }
}
