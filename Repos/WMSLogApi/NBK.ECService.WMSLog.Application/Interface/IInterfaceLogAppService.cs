using Abp.Application.Services;
using NBK.ECService.WMSLog.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Application.Interface
{
    public interface IInterfaceLogAppService : IApplicationService
    {
        void WriteInterfaceLog(InterfaceLogDto interfaceLogDto);

        LogStatisticBaseDto GetHomePageInterfaceLogStatistic(int systemId, int latestDays);

        InterfaceLogDto GetInterfaceLogViewDto(Guid sysId);

        Pages<InterfaceLogListDto> GetInterfaceLogList(int systemId, InterfaceLogQuery interfaceLogQuery);

        bool InvokeInterfaceLogApi(int systemId, InterfaceLogQuery interfaceLogQuery);

        bool InsertOutbound(int systemId, InterfaceLogQuery interfaceLogQuery);
    }
}
