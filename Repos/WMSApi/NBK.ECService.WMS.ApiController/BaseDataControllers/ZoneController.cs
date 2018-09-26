using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMS.ApiController.BaseDataControllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/Zone")]
    [AccessLog]
    public class ZoneController : AbpApiController
    {
        private IZoneAppService _zoneAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zoneAppService"></param>
        public ZoneController(IZoneAppService zoneAppService)
        {
            this._zoneAppService = zoneAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void ZoneApi() { }

        /// <summary>
        /// 获取储区列表
        /// </summary>
        /// <param name="zoneQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetZoneList")]
        public Pages<ZoneDto> GetZoneList(ZoneQuery zoneQuery)
        {
            return _zoneAppService.GetZoneList(zoneQuery);
        }

        /// <summary>
        /// 新增储区
        /// </summary>
        /// <param name="zoneDto"></param>
        /// <returns></returns>
        [HttpPost, Route("AddZone")]
        public Guid AddZone(ZoneDto zoneDto)
        {
            return _zoneAppService.AddZone(zoneDto);
        }

        /// <summary>
        /// 根据Id获取储区
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetZoneById")]
        public ZoneDto GetZoneById(Guid sysId,Guid warehouseSydId)
        {
            return _zoneAppService.GetZoneById(sysId, warehouseSydId);
        }

        /// <summary>
        /// 编辑储区
        /// </summary>
        /// <param name="zoneDto"></param>
        [HttpPost, Route("EditZone")]
        public void EditZone(ZoneDto zoneDto)
        {
            _zoneAppService.EditZone(zoneDto);
        }

        /// <summary>
        /// 删除储区
        /// </summary>
        /// <param name="sysIds"></param>
        [HttpPost, Route("DeleteZone")]
        public void DeleteZone(List<Guid> sysIds, Guid warehouseSydId)
        {
            _zoneAppService.DeleteZone(sysIds, warehouseSydId);
        }

        /// <summary>
        /// 获取储区数下拉列表据源
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <returns></returns>
        [HttpGet, Route("GetSelectZone")]
        public List<SelectItem> GetSelectZone(Guid? warehouseSysId, string zoneCode = null)
        {
            return _zoneAppService.GetSelectZone(warehouseSysId,zoneCode);
        }
    }
}
