using Abp.Application.Services;
using NBK.ECService.WMSLog.Application.Interface;
using NBK.ECService.WMSLog.Model.Models;
using NBK.ECService.WMSLog.Repository.Interface;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSLog.DTO;

namespace NBK.ECService.WMSLog.Application
{
    public class AccessLogAppService : ApplicationService, IAccessLogAppService
    {
        private ILogCrudRepository _logCrudRepository = null;
        private IAccessLogRepository<Guid> _accessLogRepository = null;

        public AccessLogAppService(ILogCrudRepository logCrudRepository, IAccessLogRepository<Guid> accessLogRepository)
        {
            _logCrudRepository = logCrudRepository;
            _accessLogRepository = accessLogRepository;
        }

        public void WriteAccessLog(AccessLogDto accessLogDto)
        {
            accessLogDto.create_date = DateTime.Now;
            _logCrudRepository.Insert(accessLogDto.JTransformTo<access_log>());
        }

        public LogStatisticBaseDto GetHomePageAccessLogStatistic(int systemId, int latestDays)
        {
            DateTime currentDate = DateTime.Now.Date;
            DateTime endDate = currentDate.AddDays(1).AddMilliseconds(-1);
            DateTime startDate = currentDate.AddDays(-latestDays + 1);
            LogStatisticBaseDto rsp = _accessLogRepository.GetHomePageAccessLogStatistic(systemId, startDate, endDate);

            DateTime sparklineStartDate = currentDate.AddDays(-rsp.PastDays + 1);
            DateTime sparklineEndDate = currentDate.AddDays(1).AddMilliseconds(-1);

            IQueryable<access_log> accessLogList = _logCrudRepository.GetAll<access_log>().Where(p => p.system_id == systemId && sparklineStartDate <= p.create_date && p.create_date <= sparklineEndDate);
            for (int i = 0; i < rsp.PastDays; i++)
            {
                DateTime fromDate = currentDate.AddDays(-i);
                DateTime toDate = currentDate.AddDays(-i + 1).AddMilliseconds(-1);
                rsp.PastDaysCount[i] = accessLogList.Where(p => fromDate <= p.create_date && p.create_date <= toDate).Count();
            }
            rsp.PastDaysCount = rsp.PastDaysCount.Reverse().ToArray();
            return rsp;
        }

        public AccessLogDto GetAccessLogViewDto(Guid sysId)
        {
            return _logCrudRepository.Get<access_log>(sysId).JTransformTo<AccessLogDto>();
        }

        public Pages<AccessLogListDto> GetAccessLogList(int systemId, AccessLogQuery accessLogQuery)
        {
            var lambda = Wheres.Lambda<access_log>();
            lambda = lambda.And(p => p.system_id == systemId);
            if (!accessLogQuery.DescrSearch.IsNull())
            {
                lambda = lambda.And(p => p.app_service.Contains(accessLogQuery.DescrSearch));
            }
            if (accessLogQuery.FlagSearch.HasValue)
            {
                lambda = lambda.And(p => p.flag == accessLogQuery.FlagSearch.Value);
            }
            if (accessLogQuery.CreateDateFromSearch.HasValue && accessLogQuery.CreateDateToSearch.HasValue)
            {
                accessLogQuery.CreateDateToSearch = accessLogQuery.CreateDateToSearch.Value.AddDays(1).AddMilliseconds(-1);
                lambda = lambda.And(p => accessLogQuery.CreateDateFromSearch.Value <= p.create_date && p.create_date <= accessLogQuery.CreateDateToSearch.Value);
            }

            accessLogQuery.iTotalDisplayRecords = _logCrudRepository.Count(lambda);
            var logs = _logCrudRepository.GetQuery(lambda).OrderByDescending(p => p.create_date).Skip(accessLogQuery.iDisplayStart).Take(accessLogQuery.iDisplayLength).Select(p =>
                new AccessLogListDto
                {
                    LogType = 400,
                    SysId = p.SysId,
                    Descr = p.app_service,
                    IsSuccess = p.flag,
                    ElapsedTime = p.elapsed_time,
                    CreateDate = p.create_date,
                    UserId = p.user_id,
                    UserName = p.user_name
                });
            return _logCrudRepository.ConvertPages(logs, accessLogQuery);
        }
    }
}
