using Schubert.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMSBussinessApi.Dto.BaseData;

namespace WMSBussinessApi.Repository
{
    public interface IBaseRep : IDependency
    {
        void SetConnection(string connectionString);

        List<WarehouseDto> GetAllWarehouseInfo();
    }
}
