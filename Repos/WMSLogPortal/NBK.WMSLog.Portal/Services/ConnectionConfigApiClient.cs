using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace NBK.WMSLog.Portal.Services
{
    public class ConnectionConfigApiClient
    {
        private static readonly ConnectionConfigApiClient instance = new ConnectionConfigApiClient();

        private ConnectionConfigApiClient() { }

        public static ConnectionConfigApiClient GetInstance() { return instance; }


        public ApiResponse<List<ConnectionStringDto>> GetAllWarehouseInfo(CoreQuery query)
        {
            return ApiClient.Get<List<ConnectionStringDto>>(PublicConst.WmsLogApiUrl, "/ConnectionConfig/GetAllWarehouseInfo", query);
        }

        public ApiResponse<ConnectionStringDto> GetConfig(CoreQuery query, string warehouseId)
        {
            query.ParmsObj = new { warehouseSysId = warehouseId };
            return ApiClient.Post<ConnectionStringDto>(PublicConst.WmsLogApiUrl, "/ConnectionConfig/GetConfig", query);
        }


        public ApiResponse<bool> UpdateWarehouseConnectionString(CoreQuery query, string warehouseId, string connectionString, string connectionStringRead)
        {
            query.ParmsObj = new { warehouseSysId = warehouseId, connectionString = connectionString, connectionStringRead = connectionStringRead };
            return ApiClient.Post<bool>(PublicConst.WmsLogApiUrl, "/ConnectionConfig/UpdateWarehouseConnectionString", query);
        }

    }
}