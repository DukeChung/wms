using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class packMap : EntityTypeConfiguration<pack>
    {
        public packMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.PackCode)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.Descr)
                .HasMaxLength(128);

            // Table & Column Mappings
            this.ToTable("pack", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.PackCode).HasColumnName("PackCode");
            this.Property(t => t.Descr).HasColumnName("Descr");
            this.Property(t => t.FieldUom01).HasColumnName("FieldUom01");
            this.Property(t => t.FieldValue01).HasColumnName("FieldValue01");
            this.Property(t => t.Cartonize01).HasColumnName("Cartonize01");
            this.Property(t => t.Replenish01).HasColumnName("Replenish01");
            this.Property(t => t.InLabelUnit01).HasColumnName("InLabelUnit01");
            this.Property(t => t.OutLabelUnit01).HasColumnName("OutLabelUnit01");
            this.Property(t => t.FieldUom02).HasColumnName("FieldUom02");
            this.Property(t => t.FieldValue02).HasColumnName("FieldValue02");
            this.Property(t => t.Cartonize02).HasColumnName("Cartonize02");
            this.Property(t => t.Replenish02).HasColumnName("Replenish02");
            this.Property(t => t.InLabelUnit02).HasColumnName("InLabelUnit02");
            this.Property(t => t.OutLabelUnit02).HasColumnName("OutLabelUnit02");
            this.Property(t => t.FieldUom03).HasColumnName("FieldUom03");
            this.Property(t => t.FieldValue03).HasColumnName("FieldValue03");
            this.Property(t => t.Cartonize03).HasColumnName("Cartonize03");
            this.Property(t => t.Replenish03).HasColumnName("Replenish03");
            this.Property(t => t.InLabelUnit03).HasColumnName("InLabelUnit03");
            this.Property(t => t.OutLabelUnit03).HasColumnName("OutLabelUnit03");
            this.Property(t => t.QueryLabelUnit01).HasColumnName("QueryLabelUnit01");
            this.Property(t => t.QueryLabelUnit02).HasColumnName("QueryLabelUnit02");
            this.Property(t => t.QueryLabelUnit03).HasColumnName("QueryLabelUnit03");
            this.Property(t => t.UPC01).HasColumnName("UPC01");
            this.Property(t => t.UPC02).HasColumnName("UPC02");
            this.Property(t => t.UPC03).HasColumnName("UPC03");
            this.Property(t => t.FieldUom04).HasColumnName("FieldUom04");
            this.Property(t => t.FieldValue04).HasColumnName("FieldValue04");
            this.Property(t => t.UPC04).HasColumnName("UPC04");
            this.Property(t => t.FieldUom05).HasColumnName("FieldUom05");
            this.Property(t => t.FieldValue05).HasColumnName("FieldValue05");
            this.Property(t => t.UPC05).HasColumnName("UPC05");
            this.Property(t => t.Source).HasColumnName("Source");
        }
    }
}
