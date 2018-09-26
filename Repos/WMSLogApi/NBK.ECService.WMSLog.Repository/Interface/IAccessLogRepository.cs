using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Repository.Interface
{
    public interface IAccessLogRepository<T> : ILogCrudRepository
    {
        LogStatisticBaseDto GetHomePageAccessLogStatistic(int systemId, DateTime startDate, DateTime endDate);
    }
}
