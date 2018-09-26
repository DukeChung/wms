using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSLog.Model.Models.Mapping
{
    public class business_logMap : EntityTypeConfiguration<business_log>
    {
        public business_logMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.SysId)
                .IsRequired();

            this.Property(t => t.access_log_sysId);

            this.Property(t => t.doc_sysId);

            this.Property(t => t.doc_order)
                .HasMaxLength(64);

            this.Property(t => t.business_type)
                .HasMaxLength(128);

            this.Property(t => t.business_name)
                .HasMaxLength(128);

            this.Property(t => t.business_operation)
                .HasMaxLength(128);

            this.Property(t => t.user_id)
                .HasMaxLength(64);

            this.Property(t => t.user_name)
                .HasMaxLength(32);

            this.Property(t => t.descr)
                .HasMaxLength(1024);

            this.Property(t => t.system_id);

            // Table & Column Mappings
            this.ToTable("business_log", "nbk_wms_log");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.access_log_sysId).HasColumnName("access_log_sysId");
            this.Property(t => t.doc_sysId).HasColumnName("doc_sysId");
            this.Property(t => t.doc_order).HasColumnName("doc_order");
            this.Property(t => t.business_type).HasColumnName("business_type");
            this.Property(t => t.business_name).HasColumnName("business_name");
            this.Property(t => t.business_operation).HasColumnName("business_operation");
            this.Property(t => t.user_id).HasColumnName("user_id");
            this.Property(t => t.user_name).HasColumnName("user_name");
            this.Property(t => t.descr).HasColumnName("descr");
            this.Property(t => t.request_json).HasColumnName("request_json");
            this.Property(t => t.old_json).HasColumnName("old_json");
            this.Property(t => t.new_json).HasColumnName("new_json");
            this.Property(t => t.create_date).HasColumnName("create_date");
            this.Property(t => t.flag).HasColumnName("flag");
            this.Property(t => t.system_id).HasColumnName("system_id");

            // Relationships
            this.HasOptional(t => t.access_log)
                .WithMany(t => t.business_log)
                .HasForeignKey(d => d.access_log_sysId);

        }
    }
}
