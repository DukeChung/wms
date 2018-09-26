using System.Data.Entity;
using Abp.EntityFramework;
using NBK.ECService.WMSLog.Model.Models;
using NBK.ECService.WMSLog.Model.Models.Mapping;
namespace NBK.ECService.WMSLog.Model
{
    public class NBK_WMS_Context : AbpDbContext
    {
        public NBK_WMS_Context() : base("nbk_wms_Context") { }

        public DbSet<warehouse> warehouses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new warehouseMap());
        }
    }
}
