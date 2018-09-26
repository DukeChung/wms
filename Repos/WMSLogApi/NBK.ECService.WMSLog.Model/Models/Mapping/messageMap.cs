using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Model.Models.Mapping
{
    public class messageMap : EntityTypeConfiguration<message>
    {
        public messageMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Table & Column Mappings
            this.ToTable("message", "nbk_wms_log");
            this.Property(t => t.SysId).HasColumnName("sysId");
            this.Property(t => t.system_id).HasColumnName("system_id");
            this.Property(t => t.message_type).HasColumnName("message_type");
            this.Property(t => t.create_date).HasColumnName("create_date");
            this.Property(t => t.create_user_id).HasColumnName("create_user_id");
            this.Property(t => t.create_user_name).HasColumnName("create_user_name");
            this.Property(t => t.content).HasColumnName("content");
            this.Property(t => t.start_time).HasColumnName("start_time");
            this.Property(t => t.end_time).HasColumnName("end_time");
            this.Property(t => t.groups).HasColumnName("groups");
            this.Property(t => t.receive_user_id).HasColumnName("receive_user_id");
            this.Property(t => t.receive_user_name).HasColumnName("receive_user_name");
            this.Property(t => t.receive_date).HasColumnName("receive_date");
            this.Property(t => t.status).HasColumnName("status");
        }
    }
}
