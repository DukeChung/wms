using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class warehouseMap : EntityTypeConfiguration<warehouse>
    {
        public warehouseMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.Address)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.Contacts)
                .IsRequired()
                .HasMaxLength(16);

            this.Property(t => t.Telephone)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.URL)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.OtherId)
                .IsRequired()
                .HasMaxLength(64);

            this.Property(t => t.UpdateUserName)
       .HasMaxLength(32);

            this.Property(t => t.CreateUserName)
            .HasMaxLength(32);

            this.Property(t => t.WareHouseArea)            .HasMaxLength(32);
            this.Property(t => t.WareHouseProperty)            .HasMaxLength(32);
            this.Property(t => t.ConnectionString)
      .HasMaxLength(256);
            this.Property(t => t.ConnectionStringRead)
.HasMaxLength(256);
            



            // Table & Column Mappings
            this.ToTable("warehouse", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.Contacts).HasColumnName("Contacts");
            this.Property(t => t.Telephone).HasColumnName("Telephone");
            this.Property(t => t.URL).HasColumnName("URL");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.OtherId).HasColumnName("OtherId");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.ConnectionString).HasColumnName("ConnectionString");
            this.Property(t => t.ConnectionStringRead).HasColumnName("ConnectionStringRead");
        }
    }
}
