using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class receiptdetailMap : EntityTypeConfiguration<receiptdetail>
    {
        public receiptdetailMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Table & Column Mappings
            this.ToTable("receiptdetail", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.ReceiptSysId).HasColumnName("ReceiptSysId");
            this.Property(t => t.SkuSysId).HasColumnName("SkuSysId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.ExpectedQty).HasColumnName("ExpectedQty");
            this.Property(t => t.ReceivedQty).HasColumnName("ReceivedQty");
            this.Property(t => t.RejectedQty).HasColumnName("RejectedQty");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UOMSysId).HasColumnName("UOMSysId");
            this.Property(t => t.PackSysId).HasColumnName("PackSysId");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.ToLoc).HasColumnName("ToLoc");
            this.Property(t => t.ToLot).HasColumnName("ToLot");
            this.Property(t => t.ToLpn).HasColumnName("ToLpn");
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
            this.Property(t => t.ShelvesQty).HasColumnName("ShelvesQty");
            this.Property(t => t.ShelvesStatus).HasColumnName("ShelvesStatus");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");            this.Property(t => t.TS).HasColumnName("TS").IsConcurrencyToken();
            this.Property(t => t.IsMustLot).HasColumnName("IsMustLot");            this.Property(t => t.IsDefaultLot).HasColumnName("IsDefaultLot");
            // Relationships
            this.HasRequired(t => t.receipt)
                .WithMany(t => t.receiptdetails)
                .HasForeignKey(d => d.ReceiptSysId);

        }
    }
}
