using System.Collections.Generic;
using System.Web.Http;
using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO.System;
using NBK.ECService.WMS.Core.WebApi.Filters;

namespace NBK.ECService.WMS.ApiController.BaseDataControllers
{  /// <summary>
   /// 系统相关
   /// </summary>
    [RoutePrefix("api/BaseData/System")]
    [AccessLog]
    public class SystemController : AbpApiController
    {
        ISystemAppService _systemAppService;

        /// <summary>
        /// 系统构造
        /// </summary>
        public SystemController(ISystemAppService systemAppService)
        {
            _systemAppService = systemAppService;
        }

        [HttpGet]
        public void SystemAPI() { }


        [HttpGet, Route("GetSystemMenuList")]
        public List<MenuDto> GetSystemMenuList()
        {
            return _systemAppService.GetSystemMenuList();
        }
    }
}