using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSLog.Model.Models.Mapping
{
    public class interface_logMap : EntityTypeConfiguration<interface_log>
    {
        public interface_logMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.SysId)
                .IsRequired();

            this.Property(t => t.doc_sysId);

            this.Property(t => t.doc_order)
                .HasMaxLength(64);

            this.Property(t => t.interface_type)
                .HasMaxLength(32);

            this.Property(t => t.interface_name)
                .HasMaxLength(128);

            this.Property(t => t.user_id)
                .HasMaxLength(64);

            this.Property(t => t.user_name)
                .HasMaxLength(32);

            this.Property(t => t.descr)
                .HasMaxLength(1024);

            this.Property(t => t.elapsed_time)
                .HasPrecision(3, 10);

            this.Property(t => t.system_id);

            // Table & Column Mappings
            this.ToTable("interface_log", "nbk_wms_log");
            this.Property(t => t.doc_sysId).HasColumnName("doc_sysId");
            this.Property(t => t.doc_order).HasColumnName("doc_order");
            this.Property(t => t.interface_type).HasColumnName("interface_type");
            this.Property(t => t.interface_name).HasColumnName("interface_name");
            this.Property(t => t.user_id).HasColumnName("user_id");
            this.Property(t => t.user_name).HasColumnName("user_name");
            this.Property(t => t.descr).HasColumnName("descr");
            this.Property(t => t.request_json).HasColumnName("request_json");
            this.Property(t => t.response_json).HasColumnName("response_json");
            this.Property(t => t.create_date).HasColumnName("create_date");
            this.Property(t => t.start_time).HasColumnName("start_time");
            this.Property(t => t.end_time).HasColumnName("end_time");
            this.Property(t => t.elapsed_time).HasColumnName("elapsed_time");
            this.Property(t => t.flag).HasColumnName("flag");
            this.Property(t => t.system_id).HasColumnName("system_id");
        }
    }
}
