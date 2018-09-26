using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class purchasedetailMap : EntityTypeConfiguration<purchasedetail>
    {
        public purchasedetailMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            this.Property(t => t.UomCode)
                .IsRequired();

            this.Property(t => t.PackCode)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("purchasedetail", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.PurchaseSysId).HasColumnName("PurchaseSysId");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.SkuClassSysId).HasColumnName("SkuClassSysId");
            this.Property(t => t.UomCode).HasColumnName("UomCode");
            this.Property(t => t.UOMSysId).HasColumnName("UOMSysId");
            this.Property(t => t.PackSysId).HasColumnName("PackSysId");
            this.Property(t => t.PackCode).HasColumnName("PackCode");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.GiftQty).HasColumnName("GiftQty");
            this.Property(t => t.ReceivedQty).HasColumnName("ReceivedQty");
            this.Property(t => t.ReceivedGiftQty).HasColumnName("ReceivedGiftQty");
            this.Property(t => t.RejectedQty).HasColumnName("RejectedQty");
            this.Property(t => t.RejectedGiftQty).HasColumnName("RejectedGiftQty");
            this.Property(t => t.LastPrice).HasColumnName("LastPrice");
            this.Property(t => t.HistoryPrice).HasColumnName("HistoryPrice");
            this.Property(t => t.PurchasePrice).HasColumnName("PurchasePrice");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.OtherSkuId).HasColumnName("OtherSkuId");
            this.Property(t => t.PackFactor).HasColumnName("PackFactor");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");

            // Relationships
            this.HasRequired(t => t.purchase)
                .WithMany(t => t.purchasedetails)
                .HasForeignKey(d => d.PurchaseSysId);

        }
    }
}
