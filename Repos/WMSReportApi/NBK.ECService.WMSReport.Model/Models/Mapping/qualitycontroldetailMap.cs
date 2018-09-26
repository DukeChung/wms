using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class qualitycontroldetailMap : EntityTypeConfiguration<qualitycontroldetail>
    {
        public qualitycontroldetailMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.Loc)
                .HasMaxLength(32);

            this.Property(t => t.Lot)
                .HasMaxLength(32);

            this.Property(t => t.Lpn)
                .HasMaxLength(32);

            this.Property(t => t.LotAttr01)
                .HasMaxLength(32);

            this.Property(t => t.LotAttr02)
                .HasMaxLength(32);

            this.Property(t => t.LotAttr03)
                .HasMaxLength(32);

            this.Property(t => t.LotAttr04)
                .HasMaxLength(32);

            this.Property(t => t.LotAttr05)
                .HasMaxLength(32);

            this.Property(t => t.LotAttr06)
                .HasMaxLength(32);

            this.Property(t => t.LotAttr07)
                .HasMaxLength(32);

            this.Property(t => t.LotAttr08)
                .HasMaxLength(32);

            this.Property(t => t.LotAttr09)
                .HasMaxLength(32);

            this.Property(t => t.ExternalLot)
                .HasMaxLength(32);

            this.Property(t => t.Descr)
                .HasMaxLength(256);

            // Table & Column Mappings
            this.ToTable("qualitycontroldetail", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.QualityControlSysId).HasColumnName("QualityControlSysId");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UOMSysId).HasColumnName("UOMSysId");
            this.Property(t => t.PackSysId).HasColumnName("PackSysId");
            this.Property(t => t.Loc).HasColumnName("Loc");
            this.Property(t => t.Lot).HasColumnName("Lot");
            this.Property(t => t.Lpn).HasColumnName("Lpn");
            this.Property(t => t.LotAttr01).HasColumnName("LotAttr01");
            this.Property(t => t.LotAttr02).HasColumnName("LotAttr02");
            this.Property(t => t.LotAttr03).HasColumnName("LotAttr03");
            this.Property(t => t.LotAttr04).HasColumnName("LotAttr04");
            this.Property(t => t.LotAttr05).HasColumnName("LotAttr05");
            this.Property(t => t.LotAttr06).HasColumnName("LotAttr06");
            this.Property(t => t.LotAttr07).HasColumnName("LotAttr07");
            this.Property(t => t.LotAttr08).HasColumnName("LotAttr08");
            this.Property(t => t.LotAttr09).HasColumnName("LotAttr09");
            this.Property(t => t.ExternalLot).HasColumnName("ExternalLot");
            this.Property(t => t.ProduceDate).HasColumnName("ProduceDate");
            this.Property(t => t.ExpiryDate).HasColumnName("ExpiryDate");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.Descr).HasColumnName("Descr");
        }
    }
}
