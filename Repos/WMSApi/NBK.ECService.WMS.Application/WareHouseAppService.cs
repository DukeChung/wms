using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using FortuneLab.WebApiClient;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application
{
    public class WareHouseAppService : WMSApplicationService, IWareHouseAppService
    {
        private IWarehouseRepository _crudRepository = null;

        public WareHouseAppService(IWarehouseRepository crudRepository)
        {
            this._crudRepository = crudRepository;
        }

        /// <summary>
        /// 根据userId获取仓库信息
        /// </summary>
        /// <param name="userId"></param>
        public List<WareHouseDto> GetWareHouseByUserId(int userId)
        {
            return _crudRepository.GetWareHouseByUserId(userId);
        }


        public Pages<UserWarehouseDto> GetNoAssignedWarehouse(UserWarehouseQuery request)
        {
            return _crudRepository.GetNoAssignedWarehouse(request);
        }

        public Pages<UserWarehouseDto> GetAssignedWarehouse(UserWarehouseQuery request)
        {
            return _crudRepository.GetAssignedWarehouse(request);
        }

        public void SetAssignedWarehouse(UserWarehouseDto request)
        {
            var uw = _crudRepository.FirstOrDefault<userwarehousemapping>(p =>
                p.WarehouseSysId == request.MapWarehouseSysId && p.UserId == request.UserId);

            if (uw == null)
            {
                userwarehousemapping newUW = new userwarehousemapping()
                {
                    SysId = Guid.NewGuid(),
                    DisplayName = request.DisplayName,
                    WarehouseSysId = request.MapWarehouseSysId,
                    UserId = request.UserId,
                    CreateBy = request.CurrentUserId,
                    CreateDate = DateTime.Now,
                    CreateUserName = request.CurrentDisplayName
                };
                if (PublicConst.SyncMultiWHSwitch)
                {
                    new Task(() =>
                    {
                        ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncCreateUserwarehousemapping", method: MethodType.Post, postData: newUW);
                    }).Start();
                }
                
                _crudRepository.Insert(newUW);
                
            }
        }

        public void SetNoAssignedWarehouse(UserWarehouseDto request)
        {
            var uw = _crudRepository.FirstOrDefault<userwarehousemapping>(p =>
                p.WarehouseSysId == request.MapWarehouseSysId && p.UserId == request.UserId);

            if (uw != null)
            {
                if (PublicConst.SyncMultiWHSwitch)
                {
                    new Task(() =>
                    {
                        ApiClient.NExecute(PublicConst.WmsBizApiUrl, "DataSync/SyncDeleteUserwarehousemapping", method: MethodType.Post, postData: uw);
                    }).Start();
                }
                
                _crudRepository.Delete(uw);
                
            }
        }

        public string GetConnectionStringByWarehouseSysId(Guid WarehouseSysId, bool IsWrite)
        {
            string connStr = string.Empty;
            warehouse wh = _crudRepository.FirstOrDefault<warehouse>(p => p.SysId == WarehouseSysId);
            if (wh!=null)
            {
                if (IsWrite)
                {
                    connStr = wh.ConnectionString;
                }
                else
                {
                    connStr = wh.ConnectionStringRead;
                } 
            }
            return connStr;
        }

        public WareHouseDto GetWarehouseByOtherId(string otherId)
        {
            WareHouseDto dto = _crudRepository.GetAllList<warehouse>(o => o.OtherId == otherId).Select(o => new WareHouseDto { SysId = o.SysId, Name = o.Name, OtherId = o.OtherId }).ToList().FirstOrDefault();
            return dto;
        }
    }
}
