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
    [RoutePrefix("api/Location")]
    [AccessLog]
    public class LocationController : AbpApiController
    {
        private ILocationAppService _locationAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locationAppService"></param>
        public LocationController(ILocationAppService locationAppService)
        {
            this._locationAppService = locationAppService;
        }

        /// <summary>
        /// Location
        /// </summary>
        [HttpGet]
        public void LocationApi() { }

        /// <summary>
        /// 获取库位列表
        /// </summary>
        /// <param name="locationQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetLocationList")]
        public Pages<LocationListDto> GetLocationList(LocationQuery locationQuery)
        {
            return _locationAppService.GetLocationList(locationQuery);
        }

        /// <summary>
        /// 新增库位
        /// </summary>
        /// <param name="locationDto"></param>
        /// <returns></returns>
        [HttpPost, Route("AddLocation")]
        public Guid AddLocation(LocationDto locationDto)
        {
            return _locationAppService.AddLocation(locationDto);
        }

        /// <summary>
        /// 根据Id获取库位
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetLocationById")]
        public LocationDto GetLocationById(Guid sysId, Guid warehouseSysId)
        {
            return _locationAppService.GetLocationById(sysId, warehouseSysId);
        }

        /// <summary>
        /// 编辑库位
        /// </summary>
        /// <param name="locationDto"></param>
        [HttpPost, Route("EditLocation")]
        public void EditLocation(LocationDto locationDto)
        {
            _locationAppService.EditLocation(locationDto);
        }

        /// <summary>
        /// 删除库位
        /// </summary>
        /// <param name="sysIds"></param>
        [HttpPost, Route("DeleteLocation")]
        public void DeleteLocation(List<Guid> sysIds, Guid warehouseSysId)
        {
            _locationAppService.DeleteLocation(sysIds, warehouseSysId);
        }

        /// <summary>
        /// 获取库位下拉框
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetSelectLocation")]
        public List<SelectItem> GetSelectLocation(Guid wareHouseSysId,Guid? zoneSysId = null)
        {
            return _locationAppService.GetSelectLocation(wareHouseSysId,zoneSysId);
        }
    }
}
