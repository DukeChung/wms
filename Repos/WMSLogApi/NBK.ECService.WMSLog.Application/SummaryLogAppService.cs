using Abp.Application.Services;
using NBK.ECService.WMSLog.Application.Interface;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Model.Models;
using NBK.ECService.WMSLog.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Application
{
    public class SummaryLogAppService : ApplicationService, ISummaryLogAppService
    {
        private ILogCrudRepository _logCrudRepository = null;
        private ISummaryLogRepository<Guid> _summaryLogRepository = null;

        public SummaryLogAppService(ILogCrudRepository logCrudRepository, ISummaryLogRepository<Guid> summaryLogRepository)
        {
            _logCrudRepository = logCrudRepository;
            _summaryLogRepository = summaryLogRepository;
        }

        public List<SummaryLogDto> GetHomePageSummaryLog(int systemId, SummaryLogQuery summaryLogQuery)
        {
            return _summaryLogRepository.GetHomePageSummaryLog(systemId, summaryLogQuery);
        }

        public List<SummaryLogDto> GetHomePageMaxElapsedTimeLog(int systemId)
        {
            DateTime currentDate = DateTime.Now.Date;
            int currentDayOfWeek = (int)currentDate.DayOfWeek;
            DateTime startDate = currentDate.AddDays(-currentDayOfWeek - 6);
            DateTime endDate = currentDate.AddDays(-currentDayOfWeek + 1).AddMilliseconds(-1);
            return _summaryLogRepository.GetHomePageMaxElapsedTimeLog(systemId, startDate, endDate);
        }

        public MaxFrequencyDto GetHomePageMaxFrequencyLog(int systemId)
        {
            MaxFrequencyDto rsp = new MaxFrequencyDto { Inteval = Convert.ToInt32(ConfigurationManager.AppSettings["MaxFrequencyInteval"]) };
            DateTime currentDate = DateTime.Now.Date;
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddDays(1).AddMilliseconds(-1);
            rsp.MaxFrequencyLogDtoList = _summaryLogRepository.GetHomePageMaxFrequencyLog(systemId, -rsp.Inteval, startDate, endDate);
            return rsp;
        }

        public MaxFrequencyLogDto GetFrequencyDetailDto(int systemId, string descr)
        {
            int maxFrequencyInteval = Convert.ToInt32(ConfigurationManager.AppSettings["MaxFrequencyInteval"]);
            DateTime currentDate = DateTime.Now.Date;
            DateTime startDate = currentDate;
            DateTime endDate = currentDate.AddDays(1).AddMilliseconds(-1);
            var rsp = _summaryLogRepository.GetHomePageMaxFrequencyLog(systemId, -maxFrequencyInteval, startDate, endDate);
            var frequencyLogDto = rsp.FirstOrDefault(p => p.Descr.Equals(descr, StringComparison.OrdinalIgnoreCase));
            if (frequencyLogDto != null)
            {
                frequencyLogDto.FrequencyDetails = new List<FrequencyDetail>();
                var sysIds = frequencyLogDto.DetailLastSysIdStr.Split(',').Select(p => Guid.Parse(p)).ToList();
                var counts = frequencyLogDto.DetailCountStr.Split(',');
                var accessLogs = _logCrudRepository.GetQuery<access_log>(p => sysIds.Contains(p.SysId)).ToList();
                for (int i = 0; i < sysIds.Count(); i++)
                {
                    var accessLog = accessLogs.FirstOrDefault(p => p.SysId == sysIds[i]);
                    string duration = string.Format("{0} ~ {1}", accessLog.create_date.AddSeconds(-maxFrequencyInteval).ToString("MM/dd HH:mm:ss.ffffff"), accessLog.create_date.ToString("MM/dd HH:mm:ss.ffffff"));
                    frequencyLogDto.FrequencyDetails.Add(new FrequencyDetail { SysId = sysIds[i], CountInInteval = Convert.ToInt32(counts[i]), Duration = duration });
                }
                frequencyLogDto.FrequencyDetails = frequencyLogDto.FrequencyDetails.OrderByDescending(p => p.CountInInteval).ToList();
                return frequencyLogDto;
            }
            else
            {
                return new MaxFrequencyLogDto { FrequencyDetails = new List<FrequencyDetail>() };
            }
        }


        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="dtoQuery"></param>
        /// <returns></returns>
        public Pages<InterfaceStatisticList> GetInterfaceStatisticByPage(int systemId, InterfaceStatisticQuery dtoQuery)
        {
            return _summaryLogRepository.GetInterfaceStatisticByPage(systemId, dtoQuery);
        }
    }
}
