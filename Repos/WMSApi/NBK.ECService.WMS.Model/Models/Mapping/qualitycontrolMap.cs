using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class qualitycontrolMap : EntityTypeConfiguration<qualitycontrol>
    {
        public qualitycontrolMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Table & Column Mappings
            this.ToTable("qualitycontrol", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.QCOrder).HasColumnName("QCOrder");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.QCType).HasColumnName("QCType");
            this.Property(t => t.ExternOrderId).HasColumnName("ExternOrderId");
            this.Property(t => t.DocOrder).HasColumnName("DocOrder");
            this.Property(t => t.QCUserName).HasColumnName("QCUserName");
            this.Property(t => t.QCDate).HasColumnName("QCDate");
            this.Property(t => t.Descr).HasColumnName("Descr");
        }
    }
}
