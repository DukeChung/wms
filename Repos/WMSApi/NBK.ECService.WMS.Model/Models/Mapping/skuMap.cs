using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class skuMap : EntityTypeConfiguration<sku>
    {
        public skuMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.SkuCode)
                .IsRequired();

            this.Property(t => t.SkuName)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("sku", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.SkuCode).HasColumnName("SkuCode");
            this.Property(t => t.SkuName).HasColumnName("SkuName");
            this.Property(t => t.SkuClassSysId).HasColumnName("SkuClassSysId");
            this.Property(t => t.ShelfLifeCodeType).HasColumnName("ShelfLifeCodeType");
            this.Property(t => t.SkuDescr).HasColumnName("SkuDescr");
            this.Property(t => t.ShelfLifeOnReceiving).HasColumnName("ShelfLifeOnReceiving");
            this.Property(t => t.ShelfLife).HasColumnName("ShelfLife");
            this.Property(t => t.PackSysId).HasColumnName("PackSysId");
            this.Property(t => t.DaysToExpire).HasColumnName("DaysToExpire");
            this.Property(t => t.LotTemplateSysId).HasColumnName("LotTemplateSysId");
            this.Property(t => t.ShelfLifeIndicator).HasColumnName("ShelfLifeIndicator");
            this.Property(t => t.Length).HasColumnName("Length");
            this.Property(t => t.Width).HasColumnName("Width");
            this.Property(t => t.Height).HasColumnName("Height");
            this.Property(t => t.Cube).HasColumnName("Cube");
            this.Property(t => t.NetWeight).HasColumnName("NetWeight");
            this.Property(t => t.GrossWeight).HasColumnName("GrossWeight");
            this.Property(t => t.CostPrice).HasColumnName("CostPrice");
            this.Property(t => t.SalePrice).HasColumnName("SalePrice");
            this.Property(t => t.Fresh).HasColumnName("Fresh");
            this.Property(t => t.FragileArticles).HasColumnName("FragileArticles");
            this.Property(t => t.Image).HasColumnName("Image");
            this.Property(t => t.Color).HasColumnName("Color");
            this.Property(t => t.Style).HasColumnName("Style");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.OtherId).HasColumnName("OtherId");
            this.Property(t => t.UPC).HasColumnName("UPC");
            this.Property(t => t.OtherUPC1).HasColumnName("OtherUPC1");
            this.Property(t => t.OtherUPC2).HasColumnName("OtherUPC2");
            this.Property(t => t.OtherUPC3).HasColumnName("OtherUPC3");
            this.Property(t => t.OtherUPC4).HasColumnName("OtherUPC4");
            this.Property(t => t.IsInvoices).HasColumnName("IsInvoices");
            this.Property(t => t.IsRefunds).HasColumnName("IsRefunds");
            this.Property(t => t.IsMaterial).HasColumnName("IsMaterial");
            this.Property(t => t.RecommendLoc).HasColumnName("RecommendLoc");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");

            this.Property(t => t.SpecialTypes).HasColumnName("SpecialTypes");
        }
    }
}
