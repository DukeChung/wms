using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class unitconversiontranMap : EntityTypeConfiguration<unitconversiontran>
    {
        public unitconversiontranMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.DocOrder)
                .HasMaxLength(32);
            // Properties
            this.Property(t => t.TransType)
                .HasMaxLength(8);
            // Properties
            this.Property(t => t.SourceTransType)
                .HasMaxLength(16);

            this.Property(t => t.FromQty)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ToQty)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Loc)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.Lot)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.Lpn)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(8);

            this.Property(t => t.PackCode)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.CreateBy)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.UpdateBy)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("unitconversiontrans", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.DocOrder).HasColumnName("DocOrder");
            this.Property(t => t.DocSysId).HasColumnName("DocSysId");
            this.Property(t => t.DocDetailSysId).HasColumnName("DocDetailSysId");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.FromQty).HasColumnName("FromQty");
            this.Property(t => t.TransType).HasColumnName("TransType");
            this.Property(t => t.SourceTransType).HasColumnName("SourceTransType");
            this.Property(t => t.ToQty).HasColumnName("ToQty");
            this.Property(t => t.Loc).HasColumnName("Loc");
            this.Property(t => t.Lot).HasColumnName("Lot");
            this.Property(t => t.Lpn).HasColumnName("Lpn");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.PackSysId).HasColumnName("PackSysId");
            this.Property(t => t.PackCode).HasColumnName("PackCode");
            this.Property(t => t.FromUOMSysId).HasColumnName("FromUOMSysId");
            this.Property(t => t.ToUOMSysId).HasColumnName("ToUOMSysId");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
        }
    }
}
