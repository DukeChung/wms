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

namespace NBK.ECService.WMS.Application
{
    public class ZoneAppService : WMSApplicationService, IZoneAppService
    {
        private ICrudRepository _crudRepository = null;

        public ZoneAppService(ICrudRepository crudRepository)
        {
            this._crudRepository = crudRepository;
        }

        /// <summary>
        /// 获取储区列表
        /// </summary>
        /// <param name="zoneQuery"></param>
        /// <returns></returns>
        public Pages<ZoneDto> GetZoneList(ZoneQuery zoneQuery)
        {
            _crudRepository.ChangeDB(zoneQuery.WarehouseSysId);
            var lambda = Wheres.Lambda<zone>();
            if (zoneQuery != null)
            {
                lambda = lambda.And(p => p.WarehouseSysId == zoneQuery.WarehouseSysId);
                if (!zoneQuery.ZoneCodeSearch.IsNull())
                {
                    lambda = lambda.And(p => p.ZoneCode == zoneQuery.ZoneCodeSearch.Trim());
                }
                if (zoneQuery.IsActiveSearch.HasValue)
                {
                    lambda = lambda.And(p => p.IsActive == zoneQuery.IsActiveSearch.Value);
                }
            }
            return _crudRepository.GetQueryableByPage<zone, ZoneDto>(zoneQuery, lambda);
        }

        /// <summary>
        /// 新增储区
        /// </summary>
        /// <param name="zoneDto"></param>
        /// <returns></returns>
        public Guid AddZone(ZoneDto zoneDto)
        {
            _crudRepository.ChangeDB(zoneDto.WarehouseSysId);
            if (_crudRepository.GetQuery<zone>(p => p.ZoneCode == zoneDto.ZoneCode && p.WarehouseSysId == zoneDto.WarehouseSysId).FirstOrDefault() != null)
            {
                throw new Exception("储区已存在");
            }
            zoneDto.SysId = Guid.NewGuid();
            zoneDto.CreateDate = DateTime.Now;
            zoneDto.UpdateDate = DateTime.Now;
            return _crudRepository.InsertAndGetId(zoneDto.JTransformTo<zone>());
        }

        /// <summary>
        /// 根据Id获取储区
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ZoneDto GetZoneById(Guid sysId,Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            return _crudRepository.FirstOrDefault<zone>(sysId).JTransformTo<ZoneDto>();
        }

        /// <summary>
        /// 编辑储区
        /// </summary>
        /// <param name="zoneDto"></param>
        public void EditZone(ZoneDto zoneDto)
        {
            _crudRepository.ChangeDB(zoneDto.WarehouseSysId);
            if (_crudRepository.GetQuery<zone>(p => p.SysId != zoneDto.SysId && p.ZoneCode == zoneDto.ZoneCode && p.WarehouseSysId == zoneDto.WarehouseSysId).FirstOrDefault() != null)
            {
                throw new Exception("储区已存在");
            }
            zoneDto.UpdateDate = DateTime.Now;
            _crudRepository.Update(zoneDto.JTransformTo<zone>());
        }

        /// <summary>
        /// 删除储区
        /// </summary>
        /// <param name="sysIds"></param>
        public void DeleteZone(List<Guid> sysIds, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var locationList = _crudRepository.GetAllList<location>(p => sysIds.Contains(p.ZoneSysId.Value));
            if (locationList != null && locationList.Any())
            {
                throw new Exception("该储区已经被货位使用，请先删除货位。");
            }
            _crudRepository.Delete<zone>(sysIds);
        }

        public List<SelectItem> GetSelectZone(Guid? warehouseSydId , string zoneCode)
        {
            _crudRepository.ChangeDB(warehouseSydId.Value);
            var zoneList = _crudRepository.GetAllList<zone>();
            if (!string.IsNullOrEmpty(zoneCode))
            {
                zoneList = zoneList.Where(p => p.ZoneCode.Contains(zoneCode)).ToList();
            }
            if (warehouseSydId.HasValue)
            {
                zoneList = zoneList.Where(p => p.WarehouseSysId == warehouseSydId).ToList();
            }
            return zoneList.Select(p => new SelectItem { Text = p.ZoneCode, Value = p.SysId.ToString() }).ToList() ;
        }
    }
}
