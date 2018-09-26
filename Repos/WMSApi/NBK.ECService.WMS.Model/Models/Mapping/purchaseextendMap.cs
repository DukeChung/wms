using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class purchaseextendMap : EntityTypeConfiguration<purchaseextend>
    {
        public purchaseextendMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Table & Column Mappings
            this.ToTable("purchaseextend", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.PurchaseSysId).HasColumnName("PurchaseSysId");
            this.Property(t => t.PlatformOrderId).HasColumnName("PlatformOrderId");
            this.Property(t => t.CustomerName).HasColumnName("CustomerName");
            this.Property(t => t.ReturnContact).HasColumnName("ReturnContact");
            this.Property(t => t.ShippingAddress).HasColumnName("ShippingAddress");
            this.Property(t => t.ExpressCompany).HasColumnName("ExpressCompany");
            this.Property(t => t.ExpressNumber).HasColumnName("ExpressNumber");
            this.Property(t => t.ReturnTime).HasColumnName("ReturnTime");
            this.Property(t => t.ReturnReason).HasColumnName("ReturnReason"); 
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.ServiceStationName).HasColumnName("ServiceStationName");
            this.Property(t => t.ServiceStationCode).HasColumnName("ServiceStationCode");
        }
    }
}
