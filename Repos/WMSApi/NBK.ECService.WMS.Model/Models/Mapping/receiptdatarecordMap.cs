using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class receiptdatarecordMap : EntityTypeConfiguration<receiptdatarecord>
    {
        public receiptdatarecordMap()
        {
            // Table & Column Mappings
            this.ToTable("receiptdatarecord", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.ReceiptSysId).HasColumnName("ReceiptSysId");
            this.Property(t => t.ReceiptOrder).HasColumnName("ReceiptOrder");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.GiftQty).HasColumnName("GiftQty");
            this.Property(t => t.RejectedQty).HasColumnName("RejectedQty");
            this.Property(t => t.GiftRejectedQty).HasColumnName("GiftRejectedQty");
            this.Property(t => t.AdjustmentQty).HasColumnName("AdjustmentQty");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
        }
    }
}
