using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class invtranMap : EntityTypeConfiguration<invtran>
    {
        public invtranMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.SkuCode)
                .IsRequired();

            this.Property(t => t.TransType)
                .IsRequired();

            this.Property(t => t.SourceTransType)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("invtrans", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.DocOrder).HasColumnName("DocOrder");
            this.Property(t => t.DocSysId).HasColumnName("DocSysId");
            this.Property(t => t.DocDetailSysId).HasColumnName("DocDetailSysId");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.SkuCode).HasColumnName("SkuCode");
            this.Property(t => t.TransType).HasColumnName("TransType");
            this.Property(t => t.SourceTransType).HasColumnName("SourceTransType");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.Loc).HasColumnName("Loc");
            this.Property(t => t.Lot).HasColumnName("Lot");
            this.Property(t => t.Lpn).HasColumnName("Lpn");
            this.Property(t => t.ToLoc).HasColumnName("ToLoc");
            this.Property(t => t.ToLot).HasColumnName("ToLot");
            this.Property(t => t.ToLpn).HasColumnName("ToLpn");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.LotAttr01).HasColumnName("LotAttr01");
            this.Property(t => t.LotAttr02).HasColumnName("LotAttr02");
            this.Property(t => t.LotAttr04).HasColumnName("LotAttr04");
            this.Property(t => t.LotAttr03).HasColumnName("LotAttr03");
            this.Property(t => t.LotAttr05).HasColumnName("LotAttr05");
            this.Property(t => t.LotAttr06).HasColumnName("LotAttr06");
            this.Property(t => t.LotAttr07).HasColumnName("LotAttr07");
            this.Property(t => t.LotAttr08).HasColumnName("LotAttr08");
            this.Property(t => t.LotAttr09).HasColumnName("LotAttr09");
            this.Property(t => t.ExternalLot).HasColumnName("ExternalLot");
            this.Property(t => t.ProduceDate).HasColumnName("ProduceDate");
            this.Property(t => t.ExpiryDate).HasColumnName("ExpiryDate");
            this.Property(t => t.ReceivedDate).HasColumnName("ReceivedDate");
            this.Property(t => t.PackSysId).HasColumnName("PackSysId");
            this.Property(t => t.PackCode).HasColumnName("PackCode");
            this.Property(t => t.UOMSysId).HasColumnName("UOMSysId");
            this.Property(t => t.UOMCode).HasColumnName("UOMCode");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
        }
    }
}
