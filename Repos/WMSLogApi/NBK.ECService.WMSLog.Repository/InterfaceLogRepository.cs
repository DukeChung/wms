using NBK.ECService.WMSLog.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Model;
using Abp.EntityFramework;
using MySql.Data.MySqlClient;

namespace NBK.ECService.WMSLog.Repository
{
    public class InterfaceLogRepository<T> : LogCrudRepository, IInterfaceLogRepository<T>
    {
        public InterfaceLogRepository(IDbContextProvider<NBK_WMS_LogContext> dbContextProvider) : base(dbContextProvider) { }

        public LogStatisticBaseDto GetHomePageInterfaceLogStatistic(int systemId, DateTime startDate, DateTime endDate)
        {
            const string sql = @"SELECT COUNT(1) AS StatisticCount, 
                                  (SUM(il.elapsed_time) / COUNT(1)) AS AvgResponseTime,
                                  MIN(il.elapsed_time) AS MinResponseTime,
                                  MAX(il.elapsed_time) AS MaxResponseTime
                                FROM interface_log il
                                WHERE il.system_id = @SystemId AND il.create_date BETWEEN @StartDate AND @EndDate;";

            var query = base.Context.Database.SqlQuery<LogStatisticBaseDto>(sql,
                new MySqlParameter("@SystemId", systemId),
                new MySqlParameter("@StartDate", startDate),
                new MySqlParameter("@EndDate", endDate));
            return query.FirstOrDefault();
        }
    }
}
