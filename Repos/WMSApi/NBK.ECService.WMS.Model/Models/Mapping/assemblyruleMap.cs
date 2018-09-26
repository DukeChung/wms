using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class assemblyruleMap : EntityTypeConfiguration<assemblyrule>
    {
        public assemblyruleMap()
        {
            this.HasKey(t => t.SysId);

            this.ToTable("assemblyrule", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.MatchingLotAttr).HasColumnName("MatchingLotAttr");
            this.Property(t => t.MatchingSkuBorrowChannel).HasColumnName("MatchingSkuBorrowChannel");
            this.Property(t => t.WarehouseSysId).HasColumnName("WarehouseSysId");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.DeliverySortRules).HasColumnName("DeliverySortRules");
        }
    }
}
