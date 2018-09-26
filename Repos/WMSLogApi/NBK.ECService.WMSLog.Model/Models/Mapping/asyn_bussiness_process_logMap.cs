using System.Data.Entity.ModelConfiguration;

namespace NBK.ECService.WMSLog.Model.Models.Mapping
{
    public class asyn_bussiness_process_logMap : EntityTypeConfiguration<asyn_bussiness_process_log>
    {
        public asyn_bussiness_process_logMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            this.Property(t => t.BussinessSysId);

            this.Property(t => t.BussinessOrderNumber)
                .HasMaxLength(32);


            this.Property(t => t.BussinessType)
            .HasMaxLength(64);

            this.Property(t => t.BussinessTypeName)
                .HasMaxLength(64);

            this.Property(t => t.UserId)
                .HasMaxLength(64);

            this.Property(t => t.UserName)
                .HasMaxLength(32);

            this.Property(t => t.Descr)
                .HasMaxLength(1024);

            this.Property(t => t.SystemId);
            this.Property(t => t.WarehouseSysId);
            // Table & Column Mappings
            this.ToTable("asyn_bussiness_process_log", "nbk_wms_log");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.BussinessOrderNumber).HasColumnName("BussinessOrderNumber");
            this.Property(t => t.BussinessSysId).HasColumnName("BussinessSysId");
            this.Property(t => t.BussinessType).HasColumnName("BussinessType");
            this.Property(t => t.BussinessTypeName).HasColumnName("BussinessTypeName");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.Descr).HasColumnName("Descr");
            this.Property(t => t.IsSuccess).HasColumnName("IsSuccess");
            this.Property(t => t.RequestJson).HasColumnName("RequestJson");
            this.Property(t => t.ResponseJson).HasColumnName("ResponseJson");
            this.Property(t => t.SystemId).HasColumnName("SystemId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.WarehouseSysId).HasColumnName("WarehouseSysId");
        }
    }
}