using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class service_station_receipt_detailMap : EntityTypeConfiguration<service_station_receipt_detail>
    {
        public service_station_receipt_detailMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.OutboundSysId)
                .HasMaxLength(36);

            this.Property(t => t.OutboundOrder)
                .HasMaxLength(32);

            this.Property(t => t.CreateByName)
                .HasMaxLength(32);

            this.Property(t => t.WareHouseName)
                .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("service_station_receipt_detail", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.ServiceStationReceiptSysId).HasColumnName("ServiceStationReceiptSysId");
            this.Property(t => t.OutboundSysId).HasColumnName("OutboundSysId");
            this.Property(t => t.OutboundOrder).HasColumnName("OutboundOrder");
            this.Property(t => t.SkuQty).HasColumnName("SkuQty");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateByName).HasColumnName("CreateByName");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.WareHouseName).HasColumnName("WareHouseName");
            this.Property(t => t.OutboundType).HasColumnName("OutboundType");
            this.Property(t => t.OutboundDate).HasColumnName("OutboundDate");
        }
    }
}
