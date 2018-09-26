using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class outbounddetailMap : EntityTypeConfiguration<outbounddetail>
    {
        public outbounddetailMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Table & Column Mappings
            this.ToTable("outbounddetail", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.OutboundSysId).HasColumnName("OutboundSysId");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.Status).HasColumnName("Status");
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
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.ShippedQty).HasColumnName("ShippedQty");
            this.Property(t => t.PickedQty).HasColumnName("PickedQty");
            this.Property(t => t.AllocatedQty).HasColumnName("AllocatedQty");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.PackFactor).HasColumnName("PackFactor");
            this.Property(t => t.IsGift).HasColumnName("IsGift");
            this.Property(t => t.GiftQty).HasColumnName("GiftQty");            this.Property(t => t.ReturnQty).HasColumnName("ReturnQty");            this.Property(t => t.Memo).HasColumnName("Memo");
            // Relationships
            this.HasOptional(t => t.outbound)
                .WithMany(t => t.outbounddetails)
                .HasForeignKey(d => d.OutboundSysId);

        }
    }
}
