using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.Utility.Enum;

namespace NBK.ECService.WMS.Application
{
    /// <summary>
    /// 
    /// </summary>
    public class LocationAppService : WMSApplicationService, ILocationAppService
    {
        private ICrudRepository _crudRepository = null;
        private ILocationRepository _locationRepository = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="crudRepository"></param>
        public LocationAppService(ICrudRepository crudRepository, ILocationRepository locationRepository)
        {
            this._crudRepository = crudRepository;
            this._locationRepository = locationRepository;
        }

        /// <summary>
        /// 获取库位列表
        /// </summary>
        /// <param name="locationQuery"></param>
        /// <returns></returns>
        public Pages<LocationListDto> GetLocationList(LocationQuery locationQuery)
        {
            _crudRepository.ChangeDB(locationQuery.WarehouseSysId);
            return _locationRepository.GetLocationListByPaging(locationQuery);
        }

        /// <summary>
        /// 新增库位
        /// </summary>
        /// <param name="locationDto"></param>
        /// <returns></returns>
        public Guid AddLocation(LocationDto locationDto)
        {
            _crudRepository.ChangeDB(locationDto.WarehouseSysId);
            if (_crudRepository.GetQuery<location>(p => p.Loc == locationDto.Loc && p.WarehouseSysId == locationDto.WarehouseSysId).FirstOrDefault() != null)
            {
                throw new Exception("货位已存在");
            }
            locationDto.SysId = Guid.NewGuid();
            locationDto.CreateDate = DateTime.Now;
            locationDto.UpdateDate = DateTime.Now;

            if (_crudRepository.GetQuery<stockfrozen>(p => p.ZoneSysId == locationDto.ZoneSysId && p.WarehouseSysId == locationDto.WarehouseSysId && p.Type == (int)FrozenType.Zone
                && p.Status == (int)FrozenStatus.Frozen).FirstOrDefault() != null)
            {
                locationDto.Status = (int)LocationStatus.Frozen;
            }
            else
            {
                locationDto.Status = (int)LocationStatus.Normal;
            }

            locationDto.Loc = locationDto.Loc.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            return _crudRepository.InsertAndGetId(locationDto.JTransformTo<location>());
        }

        /// <summary>
        /// 根据Id获取库位
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public LocationDto GetLocationById(Guid sysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            return _crudRepository.FirstOrDefault<location>(sysId).JTransformTo<LocationDto>();
        }

        /// <summary>
        /// 编辑库位
        /// </summary>
        /// <param name="locationDto"></param>
        public void EditLocation(LocationDto locationDto)
        {
            _crudRepository.ChangeDB(locationDto.WarehouseSysId);
            var location = _crudRepository.GetQuery<location>(p => p.SysId == locationDto.SysId).FirstOrDefault();

            location.LocUsage = locationDto.LocUsage;
            location.LocCategory = locationDto.LocCategory;
            location.LocFlag = locationDto.LocFlag;
            location.LocHandling = locationDto.LocHandling;
            location.LogicalLoc = locationDto.LogicalLoc;
            location.XCoord = locationDto.XCoord;
            location.YCoord = locationDto.YCoord;
            location.LocLevel = locationDto.LocLevel;
            location.Cube = locationDto.Cube;
            location.Length = locationDto.Length;
            location.Width = locationDto.Width;
            location.Height = locationDto.Height;
            location.CubicCapacity = locationDto.CubicCapacity;
            location.WeightCapacity = locationDto.WeightCapacity;
            location.IsActive = locationDto.IsActive;
            location.UpdateDate = DateTime.Now;
            location.UpdateBy = locationDto.CurrentUserId;
            location.UpdateUserName = locationDto.CurrentDisplayName;

            _crudRepository.Update(location);
        }

        /// <summary>
        /// 删除库位
        /// </summary>
        /// <param name="sysIds"></param>
        public void DeleteLocation(List<Guid> sysIds, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            List<string> locList = _crudRepository.GetAllList<location>(p => sysIds.Contains(p.SysId)).Select(p => p.Loc).ToList();
            List<invskuloc> invskulocList = _crudRepository.GetAllList<invskuloc>(p => locList.Contains(p.Loc));
            if (invskulocList != null && invskulocList.Any())
            {
                throw new Exception("该库位已经被使用，无法删除。");
            }

            var frozenLoc = _crudRepository.GetAllList<location>(p => sysIds.Contains(p.SysId) && p.Status == (int)LocationStatus.Frozen).FirstOrDefault();
            if (frozenLoc != null)
            {
                throw new Exception($"库位{frozenLoc.Loc}已经被冻结，无法删除。");
            }

            _crudRepository.Delete<location>(sysIds);
        }

        /// <summary>
        /// 根据货位查询货位数据
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public LocationDto GetLocationByLoc(string loc, Guid? warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId.Value);
            var location = _crudRepository.GetQuery<location>(x => x.Loc == loc && x.WarehouseSysId == warehouseSysId).FirstOrDefault();
            return location.JTransformTo<LocationDto>();
        }

        /// <summary>
        /// 获取库位下拉框
        /// </summary>
        /// <returns></returns>
        public List<SelectItem> GetSelectLocation(Guid wareHouseSysId, Guid? zoneSysId = null)
        {
            _crudRepository.ChangeDB(wareHouseSysId);
            var locList = _crudRepository.GetAllList<location>().Where(p => p.IsActive && p.WarehouseSysId == wareHouseSysId);
            if (zoneSysId.HasValue)
            {
                locList = locList.Where(p => p.ZoneSysId == zoneSysId.Value).ToList();
            }
            return locList.Select(p => new SelectItem { Text = p.Loc, Value = p.SysId.ToString() }).ToList();
        }
    }
}
