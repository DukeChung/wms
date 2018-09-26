using Abp.Application.Services;
using NBK.ECService.WMSLog.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Application.Interface
{
    public interface IBusinessLogAppService : IApplicationService
    {
        void WriteBusinessLog(BusinessLogDto businessLogDto);

        InboundBizLogDto GetInboundBizLogByDays(int systemId, int days);

        OutboundBizLogDto GetOutboundBizLogByDays(int systemId, int days);

        ApiProcessResultTotalDto GetApiProcessResult(int systemId);

        ApiProcessResultTotalDto GetApiProcessResult(int systemId, int secondInterval);

        BusinessLogDto GetBusinessLogViewDto(Guid sysId);

        Pages<BusinessLogListDto> GetBusinessLogList(int systemId, BusinessLogQuery businessLogQuery);
    }
}
