using System.Web.Http;
using Abp.WebApi.Controllers;
using FortuneLab.ECService.Securities.Filters;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using NBK.ECService.WMS.Core.WebApi.Filters;

namespace NBK.ECService.WMS.ApiController.BaseDataControllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/Component")]
    [AccessLog]
    public class ComponentController : AbpApiController
    {
        private IComponentAppService _componentAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentAppService"></param>
        public ComponentController(IComponentAppService componentAppService)
        {
            this._componentAppService = componentAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void ComponentApi() { }

        /// <summary>
        /// 获取组装件列表
        /// </summary>
        /// <param name="componentQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetComponentListByPaging")]
        public Pages<ComponentListDto> GetComponentListByPaging(ComponentQuery componentQuery)
        {
            return _componentAppService.GetComponentListByPaging(componentQuery);
        }

        /// <summary>
        /// 获取组装件
        /// </summary>
        /// <param name="componentQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetComponentById")]
        public ComponentDto GetComponentById(ComponentQuery componentQuery)
        {
            return _componentAppService.GetComponentById(componentQuery);
        }
    }
}
