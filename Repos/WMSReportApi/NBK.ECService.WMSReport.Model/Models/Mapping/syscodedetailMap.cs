using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class syscodedetailMap : EntityTypeConfiguration<syscodedetail>
    {
        public syscodedetailMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.SeqNo)
                .HasMaxLength(8);

            this.Property(t => t.Code)
                .HasMaxLength(32);

            this.Property(t => t.Descr)
                .HasMaxLength(128);
            this.Property(t => t.UpdateUserName)
        .HasMaxLength(32);

            this.Property(t => t.CreateUserName)
            .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("syscodedetail", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.SysCodeSysId).HasColumnName("SysCodeSysId");
            this.Property(t => t.SeqNo).HasColumnName("SeqNo");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Descr).HasColumnName("Descr");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");


            // Relationships
            this.HasRequired(t => t.syscode)
                .WithMany(t => t.syscodedetails)
                .HasForeignKey(d => d.SysCodeSysId);

        }
    }
}
