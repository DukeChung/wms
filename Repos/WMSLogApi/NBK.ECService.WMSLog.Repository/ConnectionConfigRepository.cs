using Abp.EntityFramework;
using Abp.EntityFramework.SimpleRepositories;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Model;
using NBK.ECService.WMSLog.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Repository
{
    public class ConnectionConfigRepository : EfSimpleRepositoryBase<NBK_WMS_Context, Guid>, IConnectionConfigRepository
    {
        public ConnectionConfigRepository(IDbContextProvider<NBK_WMS_Context> dbContextProvider) : base(dbContextProvider) { }

        public List<ConnectionStringDto> GetAllWarehouseInfo()
        {
            Dictionary<int, string> dicReturn = new Dictionary<int, string>();
            string sql = " SELECT w.SysId,w.Name,w.ConnectionString FROM warehouse w WHERE w.IsActive=1;";
            var queryList = base.Context.Database.SqlQuery<ConnectionStringDto>(sql).AsQueryable();
            var list = queryList.ToList();
            return list;
        }

        public ConnectionStringDto GetConfig(string warehouseSysId)
        {
            string sql = string.Format(" SELECT w.SysId,w.Name,w.ConnectionString,w.ConnectionStringRead FROM warehouse w WHERE w.IsActive=1 AND w.SysId = '{0}';", warehouseSysId);
            var queryList = base.Context.Database.SqlQuery<ConnectionStringDto>(sql).AsQueryable();
            var list = queryList.ToList();
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        public bool UpdateWarehouseConnectionString(string warehouseSysId, string connectionString, string connectionStringRead)
        {
            string sql = string.Format(" UPDATE warehouse SET ConnectionString='{0}',ConnectionStringRead='{2}' WHERE SysId = '{1}';", connectionString, warehouseSysId, connectionStringRead);
            var count = Context.Database.ExecuteSqlCommand(sql);
            if (count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
