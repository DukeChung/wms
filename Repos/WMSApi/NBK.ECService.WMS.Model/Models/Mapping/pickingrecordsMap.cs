using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class pickingrecordsMap : EntityTypeConfiguration<pickingrecords>
    {
        public pickingrecordsMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Table & Column Mappings
            this.ToTable("pickingrecords", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.PickingSysId).HasColumnName("PickingSysId");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.ReceiptSysId).HasColumnName("ReceiptSysId");
            this.Property(t => t.ReceiptOrder).HasColumnName("ReceiptOrder");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.PickingNumber).HasColumnName("PickingNumber");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.PickingUserId).HasColumnName("PickingUserId");
            this.Property(t => t.PickingUserName).HasColumnName("PickingUserName");
            this.Property(t => t.PickingDate).HasColumnName("PickingDate");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
        }
    }
}
