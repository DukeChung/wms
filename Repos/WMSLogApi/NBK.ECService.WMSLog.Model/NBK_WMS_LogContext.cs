using System.Data.Entity;
using Abp.EntityFramework;
using NBK.ECService.WMSLog.Model.Models;
using NBK.ECService.WMSLog.Model.Models.Mapping;

namespace NBK.ECService.WMSLog.Model
{
    public partial class NBK_WMS_LogContext : AbpDbContext
    {
        public NBK_WMS_LogContext() : base("nbk_wms_logContext") { }

        public DbSet<access_log> access_log { get; set; }
        public DbSet<business_log> business_log { get; set; }
        public DbSet<interface_log> interface_log { get; set; }
        public DbSet<message> message { get; set; }

        public DbSet<asyn_bussiness_process_log> asyn_bussiness_process_log { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new access_logMap());
            modelBuilder.Configurations.Add(new business_logMap());
            modelBuilder.Configurations.Add(new interface_logMap());
            modelBuilder.Configurations.Add(new messageMap());
            modelBuilder.Configurations.Add(new asyn_bussiness_process_logMap());
        }
    }
}
