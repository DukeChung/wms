using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class containerMap : EntityTypeConfiguration<container>
    {
        public containerMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.ContainerName)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("container", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.ContainerName).HasColumnName("ContainerName");
            this.Property(t => t.ContainerDescr).HasColumnName("ContainerDescr");
            this.Property(t => t.Length).HasColumnName("Length");
            this.Property(t => t.Width).HasColumnName("Width");
            this.Property(t => t.Height).HasColumnName("Height");
            this.Property(t => t.Cube).HasColumnName("Cube");
            this.Property(t => t.NetWeight).HasColumnName("NetWeight");
            this.Property(t => t.CostPrice).HasColumnName("CostPrice");
            this.Property(t => t.SalePrice).HasColumnName("SalePrice");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
        }
    }
}
