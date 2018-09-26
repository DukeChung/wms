using FortuneLab.WebApiClient;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMSLog.Application.Interface;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Repository.Interface;
using NBK.ECService.WMSLog.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Application
{
    public class ConnectionConfigAppService : IConnectionConfigAppService
    {
        private IConnectionConfigRepository _iConnectionConfigRepository = null;
        public ConnectionConfigAppService(IConnectionConfigRepository iConnectionConfigRepository)
        {
            _iConnectionConfigRepository = iConnectionConfigRepository;
        }

        public List<ConnectionStringDto> GetAllWarehouseInfo()
        {
            return _iConnectionConfigRepository.GetAllWarehouseInfo();
        }

        public ConnectionStringDto GetConfig(string warehouseSysId)
        {
            var dto = _iConnectionConfigRepository.GetConfig(warehouseSysId);
            if (dto != null)
            {
                if (!string.IsNullOrEmpty(dto.ConnectionString))
                {
                    dto.ConnectionString = StringHelper.DecryptDES(dto.ConnectionString);
                }
                if (!string.IsNullOrEmpty(dto.ConnectionStringRead))
                {
                    dto.ConnectionStringRead = StringHelper.DecryptDES(dto.ConnectionStringRead);
                }
            }
            return dto;
        }

        public bool UpdateWarehouseConnectionString(string warehouseSysId, string connectionString, string connectionStringRead)
        {
            connectionString = StringHelper.EncryptDES(connectionString);
            connectionStringRead = StringHelper.EncryptDES(connectionStringRead);
            if (PublicConst.SyncMultiWHSwitch)
            {
                var warehouse = _iConnectionConfigRepository.Get<Model.Models.warehouse>(new Guid(warehouseSysId));
                warehouse.ConnectionString = connectionString;
                warehouse.ConnectionStringRead = connectionStringRead;
                var rsp = ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncUpdateWarehouse", method: MethodType.Post, postData: warehouse);
                return rsp.Success;
            }
            else
            {
                return _iConnectionConfigRepository.UpdateWarehouseConnectionString(warehouseSysId, connectionString, connectionStringRead);
            }
        }
    }
}
