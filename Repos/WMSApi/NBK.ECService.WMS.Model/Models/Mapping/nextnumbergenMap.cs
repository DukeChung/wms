using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class nextnumbergenMap : EntityTypeConfiguration<nextnumbergen>
    {
        public nextnumbergenMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.KeyName)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("nextnumbergen", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.KeyName).HasColumnName("KeyName");
            this.Property(t => t.ColumnName).HasColumnName("ColumnName");
            this.Property(t => t.Descr).HasColumnName("Descr");
            this.Property(t => t.AlphaPrefix).HasColumnName("AlphaPrefix");
            this.Property(t => t.NextNumber).HasColumnName("NextNumber");
            this.Property(t => t.AlphaSuffix).HasColumnName("AlphaSuffix");
            this.Property(t => t.LeadingZeros).HasColumnName("LeadingZeros");
            this.Property(t => t.TotalLength).HasColumnName("TotalLength");
            this.Property(t => t.IsDateBased).HasColumnName("IsDateBased");
            this.Property(t => t.IsRefToTable).HasColumnName("IsRefToTable");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.StartNumber).HasColumnName("StartNumber");
            this.Property(t => t.DateFormat).HasColumnName("DateFormat");
            this.Property(t => t.IsIncreaseLength).HasColumnName("IsIncreaseLength");
            this.Property(t => t.IsResetYear).HasColumnName("IsResetYear");
            this.Property(t => t.LetterPrefix).HasColumnName("LetterPrefix");
            this.Property(t => t.LetterSuffix).HasColumnName("LetterSuffix");
            this.Property(t => t.IsResetMonth).HasColumnName("IsResetMonth");
        }
    }
}
