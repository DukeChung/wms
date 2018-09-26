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
    /// 容器管理
    /// </summary>
    [RoutePrefix("api/BaseData/Container")]
    [AccessLogAttribute]
    public class ContainerController : AbpApiController
    {
        IContainerAppService _containerAppService;

        /// <summary>
        /// 系统构造
        /// </summary>
        public ContainerController(IContainerAppService containerAppService)
        {
            _containerAppService = containerAppService;
        }

        [HttpGet]
        public void ContainerAPI() { }

        [HttpPost, Route("GetContainerList")]
        public Pages<ContainerDto> GetContainerList(ContainerQuery query)
        {
            return _containerAppService.GetContainerList(query);
        }

        /// <summary>
        /// 批量删除， 逗号分隔sysid
        /// </summary>
        /// <param name="sysIdList"></param>
        [HttpGet, Route("DeleteContainer")]
        public void DeleteContainer(string sysIdList, Guid warehouseSysId)
        {
            _containerAppService.DeleteContainer(sysIdList, warehouseSysId);
        }

        [HttpPost, Route("AddContainer")]
        public void AddContainer(ContainerDto container)
        {
            _containerAppService.AddContainer(container);
        }

        [HttpGet, Route("GetContainerBySysId")]
        public ContainerDto GetContainerBySysId(Guid sysId, Guid warehouseSysId)
        {
            return _containerAppService.GetContainerBySysId(sysId, warehouseSysId);
        }

        [HttpPost, Route("UpdateContainer")]
        public void UpdateContainer(ContainerDto container)
        {
            _containerAppService.UpdateContainer(container);
        }

   
        [HttpGet,Route("GetContainerListByIsActive")]
        public List<ContainerDto> GetContainerListByIsActive(Guid warehouseSysId)
        {
           return  _containerAppService.GetContainerListByIsActive(warehouseSysId);
        }
    }
}
