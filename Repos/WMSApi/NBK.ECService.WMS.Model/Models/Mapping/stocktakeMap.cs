using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class stocktakeMap : EntityTypeConfiguration<stocktake>
    {
        public stocktakeMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.StockTakeOrder)
                .IsRequired();

            this.Property(t => t.CreateUserName)
                .IsRequired();

            this.Property(t => t.UpdateUserName)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("stocktake", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.StockTakeOrder).HasColumnName("StockTakeOrder");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.StockTakeType).HasColumnName("StockTakeType");
            this.Property(t => t.StartTime).HasColumnName("StartTime");
            this.Property(t => t.EndTime).HasColumnName("EndTime");
            this.Property(t => t.AssignBy).HasColumnName("AssignBy");
            this.Property(t => t.AssignUserName).HasColumnName("AssignUserName");
            this.Property(t => t.WarehouseSysId).HasColumnName("WarehouseSysId");
            this.Property(t => t.ZoneSysId).HasColumnName("ZoneSysId");
            this.Property(t => t.StartLoc).HasColumnName("StartLoc");
            this.Property(t => t.EndLoc).HasColumnName("EndLoc");
            this.Property(t => t.SkuClassSysId1).HasColumnName("SkuClassSysId1");
            this.Property(t => t.SkuClassSysId2).HasColumnName("SkuClassSysId2");
            this.Property(t => t.SkuClassSysId3).HasColumnName("SkuClassSysId3");
            this.Property(t => t.SkuClassSysId4).HasColumnName("SkuClassSysId4");
            this.Property(t => t.SkuClassSysId5).HasColumnName("SkuClassSysId5");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
        }
    }
}
