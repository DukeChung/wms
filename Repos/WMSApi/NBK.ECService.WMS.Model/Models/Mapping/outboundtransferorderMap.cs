using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMS.Model.Models.Mapping
{
    public class outboundtransferorderMap : EntityTypeConfiguration<outboundtransferorder>
    {
        public outboundtransferorderMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            this.ToTable("outboundtransferorder", "nbk_wms");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.TransferOrder).HasColumnName("TransferOrder");
            this.Property(t => t.OutboundSysId).HasColumnName("OutboundSysId");
            this.Property(t => t.OutboundOrder).HasColumnName("OutboundOrder");
            this.Property(t => t.BoxNumber).HasColumnName("BoxNumber");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUserName).HasColumnName("CreateUserName");
            this.Property(t => t.ConsigneeArea).HasColumnName("ConsigneeArea");
            this.Property(t => t.ServiceStationName).HasColumnName("ServiceStationName");
            this.Property(t => t.PreBulkPackOrder).HasColumnName("PreBulkPackOrder");
            this.Property(t => t.PreBulkPackSysId).HasColumnName("PreBulkPackSysId");
            this.Property(t => t.Qty).HasColumnName("Qty");
            this.Property(t => t.SkuQty).HasColumnName("SkuQty");
            this.Property(t => t.WareHouseSysId).HasColumnName("WareHouseSysId");
            this.Property(t => t.TransferType).HasColumnName("TransferType");

            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUserName).HasColumnName("UpdateUserName");
            this.Property(t => t.ReviewBy).HasColumnName("ReviewBy");
            this.Property(t => t.ReviewDate).HasColumnName("ReviewDate");
            this.Property(t => t.ReviewUserName).HasColumnName("ReviewUserName");
        }
    }
}
