using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class vendorMap : EntityTypeConfiguration<vendor>
    {
        public vendorMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.VendorName)
                .HasMaxLength(128);

            this.Property(t => t.VendorPhone)
                .HasMaxLength(32);

            this.Property(t => t.OtherVendorId)
                .IsRequired()
                .HasMaxLength(64);

            this.Property(t => t.VendorContacts)
                .HasMaxLength(32);
            this.Property(t => t.UpdateUserName)
       .HasMaxLength(32);

            this.Property(t => t.CreateUserName)
            .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("vendor", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.VendorName).HasColumnName("VendorName");
            this.Property(t => t.VendorPhone).HasColumnName("VendorPhone");
            this.Property(t => t.OtherVendorId).HasColumnName("OtherVendorId");
            this.Property(t => t.VendorContacts).HasColumnName("VendorContacts");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.DeliveryCount).HasColumnName("DeliveryCount");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");

        }
    }
}
