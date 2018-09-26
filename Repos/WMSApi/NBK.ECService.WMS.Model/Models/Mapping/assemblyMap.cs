using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class assemblyMap : EntityTypeConfiguration<assembly>
    {
        public assemblyMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.AssemblyOrder)
                .IsRequired();

            this.Property(t => t.Status)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PlanQty)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ActualQty)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ShelvesQty)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ShelvesStatus)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CreateBy)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CreateUserName)
                .IsRequired();

            this.Property(t => t.UpdateBy)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.UpdateUserName)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("assembly", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.AssemblyOrder).HasColumnName("AssemblyOrder");
            this.Property(t => t.ExternalOrder).HasColumnName("ExternalOrder");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.PlanProcessingDate).HasColumnName("PlanProcessingDate");
            this.Property(t => t.PlanCompletionDate).HasColumnName("PlanCompletionDate");
            this.Property(t => t.PlanQty).HasColumnName("PlanQty");
            this.Property(t => t.ActualQty).HasColumnName("ActualQty");
            this.Property(t => t.ActualProcessingDate).HasColumnName("ActualProcessingDate");
            this.Property(t => t.ActualCompletionDate).HasColumnName("ActualCompletionDate");
            this.Property(t => t.ShelvesQty).HasColumnName("ShelvesQty");
            this.Property(t => t.ShelvesStatus).HasColumnName("ShelvesStatus");
            this.Property(t => t.Source).HasColumnName("Source");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.Lot).HasColumnName("Lot");
            this.Property(t => t.Packing).HasColumnName("Packing");
            this.Property(t => t.PackWeight).HasColumnName("PackWeight");
            this.Property(t => t.PackGrade).HasColumnName("PackGrade");
            this.Property(t => t.StorageConditions).HasColumnName("StorageConditions");
            this.Property(t => t.PackSpecification).HasColumnName("PackSpecification");
            this.Property(t => t.PackDescr).HasColumnName("PackDescr");
            this.Property(t => t.Channel).HasColumnName("Channel");
            this.Property(t => t.BatchNumber).HasColumnName("BatchNumber");
        }
    }
}
