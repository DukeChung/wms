using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class invlotMap : EntityTypeConfiguration<invlot>
    {
        public invlotMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.Lot)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("invlot", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.Lot).HasColumnName("Lot");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.CaseQty).HasColumnName("CaseQty");
            this.Property(t => t.InnerPackQty).HasColumnName("InnerPackQty");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.AllocatedQty).HasColumnName("AllocatedQty");
            this.Property(t => t.PickedQty).HasColumnName("PickedQty");
            this.Property(t => t.HoldQty).HasColumnName("HoldQty");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
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
            this.Property(t => t.ReceiptDate).HasColumnName("ReceiptDate");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");            this.Property(t => t.FrozenQty).HasColumnName("FrozenQty");
        }
    }
}
