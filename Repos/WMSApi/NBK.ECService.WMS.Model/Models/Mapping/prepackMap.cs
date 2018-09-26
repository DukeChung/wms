using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class prepackMap : EntityTypeConfiguration<prepack>
    {
        public prepackMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Table & Column Mappings
            this.ToTable("prepack", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.PrePackOrder).HasColumnName("PrePackOrder");
            this.Property(t => t.StorageLoc).HasColumnName("StorageLoc");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.OutboundOrder).HasColumnName("OutboundOrder");
            this.Property(t => t.OutboundSysId).HasColumnName("OutboundSysId");
            this.Property(t => t.Source).HasColumnName("Source");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.BatchNumber).HasColumnName("BatchNumber");
            this.Property(t => t.ServiceStationName).HasColumnName("ServiceStationName");
        }
    }
}
