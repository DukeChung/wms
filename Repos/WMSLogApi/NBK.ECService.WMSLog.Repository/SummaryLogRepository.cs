using Abp.EntityFramework;
using MySql.Data.MySqlClient;
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
    public class SummaryLogRepository<T> : LogCrudRepository, ISummaryLogRepository<T>
    {
        public SummaryLogRepository(IDbContextProvider<NBK_WMS_LogContext> dbContextProvider) : base(dbContextProvider) { }

        public List<SummaryLogDto> GetHomePageSummaryLog(int systemId, SummaryLogQuery summaryLogQuery)
        {
            const string sql = @"SELECT * FROM
                                (
                                  (SELECT '访问日志' AS LogName, 400 AS LogType, al.SysId AS SysId, al.app_service AS Descr, al.flag AS IsSuccess,
                                    al.elapsed_time AS ElapsedTime, al.create_date AS CreateDate, al.user_id AS UserId, al.user_name AS UserName 
                                    FROM access_log al WHERE al.system_id = @SystemId AND (al.flag = @Flag OR @Flag IS NULL) ORDER BY al.create_date DESC LIMIT 20)
                                      UNION ALL
                                  (SELECT '业务日志' AS LogName, 500 AS LogType, bl.SysId AS SysId, bl.descr AS Descr, bl.flag AS IsSuccess,
                                    NULL AS ElapsedTime, bl.create_date AS CreateDate, bl.user_id AS UserId, bl.user_name AS UserName
                                    FROM business_log bl WHERE bl.system_id = @SystemId AND (bl.flag = @Flag OR @Flag IS NULL) ORDER BY bl.create_date DESC LIMIT 20)
                                      UNION ALL
                                  (SELECT '接口日志' AS LogName, 600 AS LogType, il.SysId AS SysId, il.interface_name AS Descr, il.flag AS IsSuccess,
                                    il.elapsed_time AS ElapsedTime, il.create_date AS CreateDate, il.user_id AS UserId, il.user_name AS UserName
                                    FROM interface_log il WHERE il.system_id = @SystemId AND (il.flag = @Flag OR @Flag IS NULL) ORDER BY il.create_date DESC LIMIT 20)  
                                ) AS A
                                WHERE (A.LogType = @LogType OR @LogType IS NULL)
                                ORDER BY A.CreateDate DESC LIMIT 20;";
            return base.Context.Database.SqlQuery<SummaryLogDto>(sql,
                new MySqlParameter("@SystemId", systemId),
                new MySqlParameter("@LogType", summaryLogQuery.LogType),
                new MySqlParameter("@Flag", summaryLogQuery.Flag)).ToList();
        }

        public List<SummaryLogDto> GetHomePageMaxElapsedTimeLog(int systemId, DateTime startDate, DateTime endDate)
        {
            const string sql = @"SELECT * FROM
                                (
                                  SELECT '访问日志' AS LogName, 400 AS LogType, al.SysId AS SysId, al.app_service AS Descr, al.flag AS IsSuccess,
                                    al.elapsed_time AS ElapsedTime, al.create_date AS CreateDate, al.user_id AS UserId, al.user_name AS UserName 
                                    FROM access_log al WHERE al.system_id = @SystemId
                                      UNION ALL
                                  SELECT '业务日志' AS LogName, 500 AS LogType, bl.SysId AS SysId, bl.descr AS Descr, bl.flag AS IsSuccess,
                                    NULL AS ElapsedTime, bl.create_date AS CreateDate, bl.user_id AS UserId, bl.user_name AS UserName
                                    FROM business_log bl WHERE bl.system_id = @SystemId
                                      UNION ALL
                                  SELECT '接口日志' AS LogName, 600 AS LogType, il.SysId AS SysId, il.interface_name AS Descr, il.flag AS IsSuccess,
                                    il.elapsed_time AS ElapsedTime, il.create_date AS CreateDate, il.user_id AS UserId, il.user_name AS UserName
                                    FROM interface_log il WHERE il.system_id = @SystemId
                                ) AS A
                                WHERE A.CreateDate BETWEEN @StartDate AND @EndDate
                                ORDER BY A.ElapsedTime DESC LIMIT 5;";

            return base.Context.Database.SqlQuery<SummaryLogDto>(sql, new MySqlParameter("@StartDate", startDate),
                new MySqlParameter("@SystemId", systemId),
                new MySqlParameter("@EndDate", endDate)).ToList();
        }

        public List<MaxFrequencyLogDto> GetHomePageMaxFrequencyLog(int systemId, int inteval, DateTime startDate, DateTime endDate)
        {
            const string sql = @"SELECT B.app_service AS Descr, COUNT(B.CountInInteval) AS TotalCount, GROUP_CONCAT(B.SysId) AS DetailLastSysIdStr, GROUP_CONCAT(B.CountInInteval) AS DetailCountStr FROM
                                (
                                  SELECT A.SysId, A.app_service, COUNT(A.DiffTime) AS CountInInteval FROM 
                                  (
                                    SELECT al2.SysId, al2.app_service, (UNIX_TIMESTAMP(al1.create_date) - UNIX_TIMESTAMP(al2.create_date)) AS DiffTime, al1.create_date AS create_date1, al2.create_date FROM 
                                    (
                                      SELECT al.SysId, al.app_service, al.create_date 
                                      FROM access_log al 
                                      WHERE al.system_id = @SystemId AND al.app_service <> 'System/GetSystemMenuList'
                                      AND al.create_date BETWEEN @StartDate AND @EndDate
                                    ) AS al1, 
                                    (
                                      SELECT al.SysId, al.app_service, al.create_date 
                                      FROM access_log al 
                                      WHERE al.system_id = @SystemId AND al.app_service <> 'System/GetSystemMenuList'
                                      AND al.create_date BETWEEN @StartDate AND @EndDate
                                    ) AS al2
                                    WHERE al1.app_service = al2.app_service
                                  ) A
                                  WHERE A.DiffTime >= @Inteval AND A.DiffTime <= 0
                                  GROUP BY A.SysId, A.app_service
                                  HAVING CountInInteval > 1
                                  ORDER BY CountInInteval DESC
                                ) B
                                GROUP BY B.app_service ORDER BY TotalCount DESC LIMIT 5;";
            return base.Context.Database.SqlQuery<MaxFrequencyLogDto>(sql,
                new MySqlParameter("@Inteval", inteval),
                new MySqlParameter("@SystemId", systemId),
                new MySqlParameter("@StartDate", startDate),
                new MySqlParameter("@EndDate", endDate)).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="dtoQuery"></param>
        /// <returns></returns>
        public Pages<InterfaceStatisticList> GetInterfaceStatisticByPage(int systemId, InterfaceStatisticQuery dtoQuery)
        {
            var countSql = new StringBuilder();
            var alc = string.Empty;
            var blc = string.Empty;
            var ilc = string.Empty;
            if (dtoQuery.CreateDateFrom.HasValue)
            {
                alc += " And  al.create_date >='" + dtoQuery.CreateDateFrom + "'";
                blc += " And  bl.create_date >='" + dtoQuery.CreateDateFrom + "'";
                ilc += " And  il.create_date >='" + dtoQuery.CreateDateFrom + "'";
            }
            if (dtoQuery.CreateDateTo.HasValue)
            {
                alc += " And  al.create_date <='" + dtoQuery.CreateDateTo + "'";
                blc += " And  bl.create_date <='" + dtoQuery.CreateDateTo + "'";
                ilc += " And  il.create_date <='" + dtoQuery.CreateDateTo + "'";

            }
            countSql.AppendFormat(@" SELECT SUM(num) FROM (
                               SELECT COUNT(*) AS num FROM access_log al
                                    WHERE  al.system_id = {0} {1}
                                    UNION ALL
                                SELECT COUNT(*) AS num FROM business_log bl
                                    WHERE  bl.system_id = {0}  {2}
                                    UNION ALL
                                SELECT COUNT(*) AS num FROM interface_log il
                                    WHERE  il.system_id = {0}  {3}) A;", systemId, alc, blc, ilc);
            var count = base.Context.Database.SqlQuery<int>(countSql.ToString()).AsQueryable().First();

            var listSql = new StringBuilder();
            listSql.AppendFormat(@"SELECT * FROM ((SELECT '访问日志'  AS LogName,
                                        400             AS LogType,
                                        al.SysId        AS SysId,
                                        al.app_service  AS Descr,
                                        al.flag         AS IsSuccess,
                                        al.elapsed_time AS ElapsedTime,
                                        al.create_date  AS CreateDate,
                                        al.user_id      AS UserId,
                                        al.user_name    AS UserName,
                                        IFNULL(CHAR_LENGTH(al.request_json),0) AS RequestLength,
                                        IFNULL( CHAR_LENGTH(al.response_json),0) AS ResponseLength
                                    FROM   access_log al
                                    WHERE  al.system_id = {0} {1}
                                    ORDER  BY al.create_date DESC
                                    LIMIT  {4},{5})
                                UNION ALL
                                (SELECT '业务日志'         AS LogName,
                                        500            AS LogType,
                                        bl.SysId       AS SysId,
                                        bl.descr       AS Descr,
                                        bl.flag        AS IsSuccess,
                                        NULL           AS ElapsedTime,
                                        bl.create_date AS CreateDate,
                                        bl.user_id     AS UserId,
                                        bl.user_name   AS UserName,
                                        IFNULL(CHAR_LENGTH(bl.request_json),0) AS RequestLength,
                                        0  AS ResponseLength
                                    FROM   business_log bl
                                    WHERE  bl.system_id = {0}  {2} 
                                    ORDER  BY bl.create_date DESC
                                    LIMIT  {4},{5})
                                UNION ALL
                                (SELECT '接口日志'            AS LogName,
                                        600               AS LogType,
                                        il.SysId          AS SysId,
                                        il.interface_name AS Descr,
                                        il.flag           AS IsSuccess,
                                        il.elapsed_time   AS ElapsedTime,
                                        il.create_date    AS CreateDate,
                                        il.user_id        AS UserId,
                                        il.user_name      AS UserName,
                                       IFNULL( CHAR_LENGTH(il.request_json),0) AS RequestLength,
                                       IFNULL( CHAR_LENGTH(il.response_json),0) AS ResponseLength
                                    FROM   interface_log il
                                    WHERE  il.system_id = {0} {3}               
                                    ORDER  BY il.create_date DESC 
                                    LIMIT  {4},{5})) AS A 
                                    ORDER  BY A.CreateDate DESC
                                    LIMIT  {4},{5};", systemId, alc, blc, ilc, dtoQuery.iDisplayStart, dtoQuery.iDisplayLength);
            var resultList = base.Context.Database.SqlQuery<InterfaceStatisticList>(listSql.ToString()).AsQueryable().ToList();
            var response = new Pages<InterfaceStatisticList>();
            response.TableResuls = new TableResults<InterfaceStatisticList>()
            {
                aaData = resultList,
                iTotalDisplayRecords = count,
                iTotalRecords = resultList.Count(),
                sEcho = dtoQuery.sEcho
            };
            return response;
        }
    }
}
