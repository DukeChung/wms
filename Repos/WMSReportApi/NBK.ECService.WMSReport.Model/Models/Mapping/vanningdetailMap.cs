using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class vanningdetailMap : EntityTypeConfiguration<vanningdetail>
    {
        public vanningdetailMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.ContainerNumber)
                .IsRequired()
                .HasMaxLength(16);

            this.Property(t => t.CarrierNumber)
                .HasMaxLength(32);
            this.Property(t => t.HandoverGroupOrder)
          .HasMaxLength(32);
            this.Property(t => t.UpdateUserName)
       .HasMaxLength(32);

            this.Property(t => t.CreateUserName)
            .HasMaxLength(32);
            // Table & Column Mappings
            this.ToTable("vanningdetail", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.VanningSysId).HasColumnName("VanningSysId");
            this.Property(t => t.ContainerSysId).HasColumnName("ContainerSysId");
            this.Property(t => t.ContainerNumber).HasColumnName("ContainerNumber");
            this.Property(t => t.CarrierSysId).HasColumnName("CarrierSysId");
            this.Property(t => t.CarrierNumber).HasColumnName("CarrierNumber");
            this.Property(t => t.Weight).HasColumnName("Weight");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.HandoverGroupOrder).HasColumnName("HandoverGroupOrder");
            this.Property(t => t.HandoverCreateDate).HasColumnName("HandoverCreateDate");
            this.Property(t => t.HandoverCreateBy).HasColumnName("HandoverCreateBy");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");

            //// Relationships
            //this.HasRequired(t => t.container)
            //    .WithMany(t => t.vanningdetails)
            //    .HasForeignKey(d => d.ContainerSysId);
            this.HasOptional(t => t.vanning)
                .WithMany(t => t.vanningdetails)
                .HasForeignKey(d => d.VanningSysId);

        }
    }
}
