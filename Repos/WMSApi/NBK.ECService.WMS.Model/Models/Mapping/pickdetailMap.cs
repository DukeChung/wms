using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class pickdetailMap : EntityTypeConfiguration<pickdetail>
    {
        public pickdetailMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            this.Property(t => t.PickDetailOrder)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("pickdetail", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.OutboundSysId).HasColumnName("OutboundSysId");
            this.Property(t => t.OutboundDetailSysId).HasColumnName("OutboundDetailSysId");
            this.Property(t => t.PickDetailOrder).HasColumnName("PickDetailOrder");
            this.Property(t => t.PickDate).HasColumnName("PickDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.UOMSysId).HasColumnName("UOMSysId");
            this.Property(t => t.PackSysId).HasColumnName("PackSysId");
            this.Property(t => t.Loc).HasColumnName("Loc");
            this.Property(t => t.Lot).HasColumnName("Lot");
            this.Property(t => t.Lpn).HasColumnName("Lpn");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");            this.Property(t => t.SourceType).HasColumnName("SourceType");

        }
    }
}
