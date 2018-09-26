using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class workuserMap : EntityTypeConfiguration<workuser>
    {
        public workuserMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            this.Property(t => t.CreateUserName)
                .IsRequired();

            this.Property(t => t.UpdateUserName)
                .IsRequired();

            this.ToTable("workuser", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.WorkUserCode).HasColumnName("WorkUserCode");
            this.Property(t => t.WorkUserName).HasColumnName("WorkUserName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.WorkType).HasColumnName("WorkType");
            this.Property(t => t.WorkStatus).HasColumnName("WorkStatus");
            this.Property(t => t.Proficiency).HasColumnName("Proficiency");
            this.Property(t => t.TS).HasColumnName("TS");
            this.Property(t => t.IsAssigned).HasColumnName("IsAssigned");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
        }
    }
}
