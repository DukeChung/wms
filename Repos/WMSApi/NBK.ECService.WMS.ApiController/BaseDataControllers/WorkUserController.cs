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
    [RoutePrefix("api/BaseData/WorkUser")]
    [AccessLog]
    public class WorkUserController : AbpApiController
    {
        private IWorkUserAppService _workUserAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workUserAppService"></param>
        public WorkUserController(IWorkUserAppService workUserAppService)
        {
            this._workUserAppService = workUserAppService;
        }

        /// <summary>
        /// 获取工单用户列表
        /// </summary>
        /// <param name="workUserQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWorkUserList")]
        public Pages<WorkUserDto> GetWorkUserList(WorkUserQuery workUserQuery)
        {
            return _workUserAppService.GetWorkUserList(workUserQuery);
        }

        /// <summary>
        /// 新增工单用户
        /// </summary>
        /// <param name="workUserDto"></param>
        [HttpPost, Route("AddWorkUser")]
        public void AddWorkUser(WorkUserDto workUserDto)
        {
            _workUserAppService.AddWorkUser(workUserDto);
        }

        /// <summary>
        /// 根据Id获取工单用户
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSydId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetWorkUserById")]
        public WorkUserDto GetWorkUserById(Guid sysId, Guid warehouseSydId)
        {
            return _workUserAppService.GetWorkUserById(sysId, warehouseSydId);
        }

        /// <summary>
        /// 获取所有启用工单用户
        /// </summary>
        /// <param name="workUserQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWorkUsers")]
        public List<WorkUserListDto> GetWorkUsers(WorkUserQuery workUserQuery)
        {
            return _workUserAppService.GetWorkUsers(workUserQuery);
        }

        /// <summary>
        /// 编辑工单用户
        /// </summary>
        /// <param name="workUserDto"></param>
        [HttpPost, Route("EditWorkUser")]
        public void EditWorkUser(WorkUserDto workUserDto)
        {
            _workUserAppService.EditWorkUser(workUserDto);
        }

        /// <summary>
        /// 删除工单用户
        /// </summary>
        /// <param name="sysIds"></param>
        /// <param name="warehouseSydId"></param>
        [HttpPost, Route("DeleteWorkUser")]
        public void DeleteWorkUser(List<Guid> sysIds, Guid warehouseSydId)
        {
            _workUserAppService.DeleteWorkUser(sysIds, warehouseSydId);
        }
    }
}
