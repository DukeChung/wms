using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class carrierMap : EntityTypeConfiguration<carrier>
    {
        public carrierMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            this.Property(t => t.OtherCarrierId)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("carrier", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.CarrierName).HasColumnName("CarrierName");
            this.Property(t => t.CarrierPhone).HasColumnName("CarrierPhone");
            this.Property(t => t.OtherCarrierId).HasColumnName("OtherCarrierId");
            this.Property(t => t.CarrierContacts).HasColumnName("CarrierContacts");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.DeliveryCount).HasColumnName("DeliveryCount");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
        }
    }
}
