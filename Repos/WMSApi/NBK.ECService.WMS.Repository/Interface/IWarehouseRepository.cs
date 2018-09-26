using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IWarehouseRepository : ICrudRepository
    {
        Pages<UserWarehouseDto> GetNoAssignedWarehouse(UserWarehouseQuery request);

        Pages<UserWarehouseDto> GetAssignedWarehouse(UserWarehouseQuery request);

        /// <summary>
        /// 根据userId获取仓库信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<WareHouseDto> GetWareHouseByUserId(int userId);
    }
}
