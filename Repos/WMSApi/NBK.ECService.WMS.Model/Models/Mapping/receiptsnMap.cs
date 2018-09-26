using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class receiptsnMap : EntityTypeConfiguration<receiptsn>
    {
        public receiptsnMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Table & Column Mappings
            this.ToTable("receiptsn", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.ReceiptSysId).HasColumnName("ReceiptSysId");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.SN).HasColumnName("SN");

            this.Property(t => t.OutboundSysId).HasColumnName("OutboundSysId");
            this.Property(t => t.PurchaseSysId).HasColumnName("purchaseSysId");
            this.Property(t => t.WarehouseSysId).HasColumnName("WarehouseSysId");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.Status).HasColumnName("status");

            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
        }
    }
}
