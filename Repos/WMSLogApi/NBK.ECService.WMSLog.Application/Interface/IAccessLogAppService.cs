using Abp.Application.Services;
using NBK.ECService.WMSLog.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Application.Interface
{
    public interface IAccessLogAppService : IApplicationService
    {
        void WriteAccessLog(AccessLogDto accessLogDto);

        LogStatisticBaseDto GetHomePageAccessLogStatistic(int systemId, int latestDays);

        AccessLogDto GetAccessLogViewDto(Guid sysId);

        Pages<AccessLogListDto> GetAccessLogList(int systemId, AccessLogQuery accessLogQuery);
    }
}
