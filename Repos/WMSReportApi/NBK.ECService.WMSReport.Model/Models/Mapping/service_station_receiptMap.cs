using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSReport.Model.Models.Mapping
{
    public class service_station_receiptMap : EntityTypeConfiguration<service_station_receipt>
    {
        public service_station_receiptMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            this.Property(t => t.ServiceStationName)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.OutboundOrder)
                .HasMaxLength(32);

            // Table & Column Mappings
            this.ToTable("service_station_receipt", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.ServiceStationName).HasColumnName("ServiceStationName");
            this.Property(t => t.ReceiptPeriod).HasColumnName("ReceiptPeriod");
            this.Property(t => t.LastReceiptDate).HasColumnName("LastReceiptDate");
            this.Property(t => t.LastReceiptPeriodSkuQty).HasColumnName("LastReceiptPeriodSkuQty");
            this.Property(t => t.LastReceiptPeriodQty).HasColumnName("LastReceiptPeriodQty");
            this.Property(t => t.NextReceiptPeriodDate).HasColumnName("NextReceiptPeriodDate");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.Longitude).HasColumnName("Longitude");
            this.Property(t => t.Latitude).HasColumnName("Latitude");
            this.Property(t => t.OutboundOrder).HasColumnName("OutboundOrder");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.TotalQty).HasColumnName("TotalQty");
        }
    }
}
