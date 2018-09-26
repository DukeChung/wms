using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class outboundruleMap : EntityTypeConfiguration<outboundrule>
    {
        public outboundruleMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Table & Column Mappings
            this.ToTable("outboundrule", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.MatchingLotAttr).HasColumnName("MatchingLotAttr");
            this.Property(t => t.WarehouseSysId).HasColumnName("WarehouseSysId");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.DeliverySortRules).HasColumnName("DeliverySortRules");
            this.Property(t => t.DeliveryIsAsyn).HasColumnName("DeliveryIsAsyn");
            this.Property(t => t.CreateOutboundIsAsyn).HasColumnName("CreateOutboundIsAsyn");
            this.Property(t => t.IsPickingSkuLoc).HasColumnName("IsPickingSkuLoc");
            this.Property(t => t.AutomaticAllocation).HasColumnName("AutomaticAllocation");
        }
    }
}