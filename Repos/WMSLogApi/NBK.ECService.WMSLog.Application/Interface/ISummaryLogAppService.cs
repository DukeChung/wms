using Abp.Application.Services;
using NBK.ECService.WMSLog.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Application.Interface
{
    public interface ISummaryLogAppService : IApplicationService
    {
        List<SummaryLogDto> GetHomePageSummaryLog(int systemId, SummaryLogQuery summaryLogQuery);

        List<SummaryLogDto> GetHomePageMaxElapsedTimeLog(int systemId);

        MaxFrequencyDto GetHomePageMaxFrequencyLog(int systemId);

        MaxFrequencyLogDto GetFrequencyDetailDto(int systemId, string descr);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="dtoQuery"></param>
        /// <returns></returns>
        Pages<InterfaceStatisticList> GetInterfaceStatisticByPage(int systemId, InterfaceStatisticQuery dtoQuery);
    }
}
