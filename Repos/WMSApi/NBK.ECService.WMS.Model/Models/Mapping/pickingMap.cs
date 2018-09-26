using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class pickingMap : EntityTypeConfiguration<picking>
    {
        public pickingMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Table & Column Mappings
            this.ToTable("picking", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.PickingOrder).HasColumnName("PickingOrder");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.ReceiptSysId).HasColumnName("ReceiptSysId");
            this.Property(t => t.ReceiptOrder).HasColumnName("ReceiptOrder");
            this.Property(t => t.PickingNumber).HasColumnName("PickingNumber");
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
