using Abp.Domain.Repositories;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Repository.Interface
{
    public interface IConnectionConfigRepository: ISimpleRepository<Guid>
    {
        List<ConnectionStringDto> GetAllWarehouseInfo();

        ConnectionStringDto GetConfig(string warehouseSysId);

        bool UpdateWarehouseConnectionString(string warehouseSysId, string connectionString,string connectionStringRead);
    }
}
