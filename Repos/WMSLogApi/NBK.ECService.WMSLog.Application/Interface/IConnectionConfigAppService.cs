using Abp.Application.Services;
using NBK.ECService.WMSLog.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Application.Interface
{
    public interface IConnectionConfigAppService: IApplicationService
    {
        List<ConnectionStringDto> GetAllWarehouseInfo();

        ConnectionStringDto GetConfig(string warehouseSysId);

        bool UpdateWarehouseConnectionString(string warehouseSysId, string connectionString,string connectionStringRead);
    }
}
