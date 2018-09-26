using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class workMap : EntityTypeConfiguration<work>
    {
        public workMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.WorkOrder)
                .IsRequired();

            this.Property(t => t.CreateUserName)
                .IsRequired();

            this.Property(t => t.UpdateUserName)
                .IsRequired();

            this.ToTable("work", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.WorkOrder).HasColumnName("WorkOrder");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.WorkType).HasColumnName("WorkType");
            this.Property(t => t.Priority).HasColumnName("Priority");

            this.Property(t => t.AppointUserId).HasColumnName("AppointUserId");
            this.Property(t => t.AppointUserName).HasColumnName("AppointUserName");
            this.Property(t => t.StartTime).HasColumnName("StartTime");
            this.Property(t => t.EndTime).HasColumnName("EndTime");
            this.Property(t => t.WorkTime).HasColumnName("WorkTime");

            this.Property(t => t.Descr).HasColumnName("Descr");
            this.Property(t => t.Source).HasColumnName("Source");
            this.Property(t => t.DocSysId).HasColumnName("DocSysId");
            this.Property(t => t.DocOrder).HasColumnName("DocOrder");
            this.Property(t => t.DocDetailSysId).HasColumnName("DocDetailSysId");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");

            this.Property(t => t.Lot).HasColumnName("Lot");
            this.Property(t => t.Lpn).HasColumnName("Lpn");
            this.Property(t => t.FromLoc).HasColumnName("FromLoc");
            this.Property(t => t.ToLoc).HasColumnName("ToLoc");
            this.Property(t => t.FromQty).HasColumnName("FromQty");
            this.Property(t => t.ToQty).HasColumnName("ToQty");
            this.Property(t => t.WarehouseSysId).HasColumnName("WarehouseSysId");

            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");



        }
    }
}
