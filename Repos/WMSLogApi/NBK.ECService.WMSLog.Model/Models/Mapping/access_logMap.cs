using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSLog.Model.Models.Mapping
{
    public class access_logMap : EntityTypeConfiguration<access_log>
    {
        public access_logMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.SysId)
                .IsRequired();

            this.Property(t => t.app_controller)
                .HasMaxLength(64);

            this.Property(t => t.app_service)
                .HasMaxLength(64);

            this.Property(t => t.user_id)
                .HasMaxLength(64);

            this.Property(t => t.user_name)
                .HasMaxLength(32);

            this.Property(t => t.descr)
                .HasMaxLength(1024);

            this.Property(t => t.ip)
                .HasMaxLength(64);

            this.Property(t => t.elapsed_time)
                .HasPrecision(3, 10);

            this.Property(t => t.system_id);
            // Table & Column Mappings
            this.ToTable("access_log", "nbk_wms_log");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.app_controller).HasColumnName("app_controller");
            this.Property(t => t.app_service).HasColumnName("app_service");
            this.Property(t => t.user_id).HasColumnName("user_id");
            this.Property(t => t.user_name).HasColumnName("user_name");
            this.Property(t => t.descr).HasColumnName("descr");
            this.Property(t => t.create_date).HasColumnName("create_date");
            this.Property(t => t.start_time).HasColumnName("start_time");
            this.Property(t => t.end_time).HasColumnName("end_time");
            this.Property(t => t.elapsed_time).HasColumnName("elapsed_time");
            this.Property(t => t.ip).HasColumnName("ip");
            this.Property(t => t.request_json).HasColumnName("request_json");
            this.Property(t => t.response_json).HasColumnName("response_json");
            this.Property(t => t.flag).HasColumnName("flag");
            this.Property(t => t.system_id).HasColumnName("system_id");
        }
    }
}
