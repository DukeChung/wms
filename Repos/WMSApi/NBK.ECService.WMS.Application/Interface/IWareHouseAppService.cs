using System.Collections.Generic;
using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IWareHouseAppService : IApplicationService
    {
        /// <summary>
        /// 根据userId获取仓库信息
        /// </summary>
        /// <param name="userId"></param>
        List<WareHouseDto> GetWareHouseByUserId(int userId);

        Pages<UserWarehouseDto> GetNoAssignedWarehouse(UserWarehouseQuery request);

        Pages<UserWarehouseDto> GetAssignedWarehouse(UserWarehouseQuery request);

        void SetAssignedWarehouse(UserWarehouseDto request);

        void SetNoAssignedWarehouse(UserWarehouseDto request);

        string GetConnectionStringByWarehouseSysId(Guid WarehouseSysId, bool IsWrite);

        WareHouseDto GetWarehouseByOtherId(string otherId);
    }
}
