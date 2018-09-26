using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class stocktransferMap : EntityTypeConfiguration<stocktransfer>
    {
        public stocktransferMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.StockTransferOrder)
                .HasMaxLength(36);

            this.Property(t => t.Descr)
                .HasMaxLength(256);

            this.Property(t => t.FromLot)
                .HasMaxLength(32);

            this.Property(t => t.ToLot)
                .HasMaxLength(32);

            this.Property(t => t.FromLoc)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.ToLoc)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.FromQty)
                .IsRequired();

            this.Property(t => t.ToQty)
                .IsRequired();

            this.Property(t => t.FromLpn)
                .HasMaxLength(32);

            this.Property(t => t.ToLpn)
                .HasMaxLength(32);

            this.Property(t => t.FromLotAttr01)
                .HasMaxLength(32);

            this.Property(t => t.FromLotAttr02)
                .HasMaxLength(32);

            this.Property(t => t.FromLotAttr04)
                .HasMaxLength(32);

            this.Property(t => t.FromLotAttr03)
                .HasMaxLength(32);

            this.Property(t => t.FromLotAttr05)
                .HasMaxLength(32);

            this.Property(t => t.FromLotAttr06)
                .HasMaxLength(32);

            this.Property(t => t.FromLotAttr07)
                .HasMaxLength(32);

            this.Property(t => t.FromLotAttr08)
                .HasMaxLength(32);

            this.Property(t => t.FromLotAttr09)
                .HasMaxLength(32);

            this.Property(t => t.FromExternalLot)
                .HasMaxLength(32);

            this.Property(t => t.ToLotAttr01)
                .HasMaxLength(32);

            this.Property(t => t.ToLotAttr02)
                .HasMaxLength(32);

            this.Property(t => t.ToLotAttr04)
                .HasMaxLength(32);

            this.Property(t => t.ToLotAttr03)
                .HasMaxLength(32);

            this.Property(t => t.ToLotAttr05)
                .HasMaxLength(32);

            this.Property(t => t.ToLotAttr06)
                .HasMaxLength(32);

            this.Property(t => t.ToLotAttr07)
                .HasMaxLength(32);

            this.Property(t => t.ToLotAttr08)
                .HasMaxLength(32);

            this.Property(t => t.ToLotAttr09)
                .HasMaxLength(32);

            this.Property(t => t.ToExternalLot)
                .HasMaxLength(32);

            this.Property(t => t.CreateUserName)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(t => t.UpdateUserName)
                .IsRequired()
                .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("stocktransfer", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.StockTransferOrder).HasColumnName("StockTransferOrder");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Descr).HasColumnName("Descr");
            this.Property(t => t.FromSkuSysId).HasColumnName("FromSkuSysId");
            this.Property(t => t.ToSkuSysId).HasColumnName("ToSkuSysId");
            this.Property(t => t.FromLot).HasColumnName("FromLot");
            this.Property(t => t.ToLot).HasColumnName("ToLot");
            this.Property(t => t.FromLoc).HasColumnName("FromLoc");
            this.Property(t => t.ToLoc).HasColumnName("ToLoc");
            this.Property(t => t.FromQty).HasColumnName("FromQty");
            this.Property(t => t.ToQty).HasColumnName("ToQty");
            this.Property(t => t.FromLpn).HasColumnName("FromLpn");
            this.Property(t => t.ToLpn).HasColumnName("ToLpn");
            this.Property(t => t.FromLotAttr01).HasColumnName("FromLotAttr01");
            this.Property(t => t.FromLotAttr02).HasColumnName("FromLotAttr02");
            this.Property(t => t.FromLotAttr04).HasColumnName("FromLotAttr04");
            this.Property(t => t.FromLotAttr03).HasColumnName("FromLotAttr03");
            this.Property(t => t.FromLotAttr05).HasColumnName("FromLotAttr05");
            this.Property(t => t.FromLotAttr06).HasColumnName("FromLotAttr06");
            this.Property(t => t.FromLotAttr07).HasColumnName("FromLotAttr07");
            this.Property(t => t.FromLotAttr08).HasColumnName("FromLotAttr08");
            this.Property(t => t.FromLotAttr09).HasColumnName("FromLotAttr09");
            this.Property(t => t.FromProduceDate).HasColumnName("FromProduceDate");
            this.Property(t => t.FromExpiryDate).HasColumnName("FromExpiryDate");
            this.Property(t => t.FromExternalLot).HasColumnName("FromExternalLot");
            this.Property(t => t.ToLotAttr01).HasColumnName("ToLotAttr01");
            this.Property(t => t.ToLotAttr02).HasColumnName("ToLotAttr02");
            this.Property(t => t.ToLotAttr04).HasColumnName("ToLotAttr04");
            this.Property(t => t.ToLotAttr03).HasColumnName("ToLotAttr03");
            this.Property(t => t.ToLotAttr05).HasColumnName("ToLotAttr05");
            this.Property(t => t.ToLotAttr06).HasColumnName("ToLotAttr06");
            this.Property(t => t.ToLotAttr07).HasColumnName("ToLotAttr07");
            this.Property(t => t.ToLotAttr08).HasColumnName("ToLotAttr08");
            this.Property(t => t.ToLotAttr09).HasColumnName("ToLotAttr09");
            this.Property(t => t.ToProduceDate).HasColumnName("ToProduceDate");
            this.Property(t => t.ToExpiryDate).HasColumnName("ToExpiryDate");
            this.Property(t => t.ToExternalLot).HasColumnName("ToExternalLot");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
        }
    }
}
