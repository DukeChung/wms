using NBK.ECService.WMSLog.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Repository.Interface
{
    public interface ISummaryLogRepository<T> : ILogCrudRepository
    {
        List<SummaryLogDto> GetHomePageSummaryLog(int systemId, SummaryLogQuery summaryLogQuery);

        List<SummaryLogDto> GetHomePageMaxElapsedTimeLog(int systemId, DateTime startDate, DateTime endDate);

        List<MaxFrequencyLogDto> GetHomePageMaxFrequencyLog(int systemId, int inteval, DateTime startDate, DateTime endDate);

        Pages<InterfaceStatisticList> GetInterfaceStatisticByPage(int systemId, InterfaceStatisticQuery dtoQuery);
    }
}
