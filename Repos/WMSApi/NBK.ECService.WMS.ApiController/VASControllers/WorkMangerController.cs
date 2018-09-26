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

namespace NBK.ECService.WMS.ApiController.VASControllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/WorkManger")]
    [AccessLog]
    public class WorkMangerController : AbpApiController
    {
        private IWorkMangerAppService _workMangerAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workMangerAppService"></param>
        public WorkMangerController(IWorkMangerAppService workMangerAppService)
        {
            _workMangerAppService = workMangerAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void WorkMangerAPI() { }

        /// <summary>
        /// 分页获取工单数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWorkByPage")]
        public Pages<WorkListDto> GetWorkByPage(WorkQueryDto request)
        {
            return _workMangerAppService.GetWorkByPage(request);
        }


        /// <summary>
        /// 获取工单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetWorkBySysId")]
        public WorkDetailDto GetWorkBySysId(Guid sysId, Guid warehouseSysId)
        {
            return _workMangerAppService.GetWorkBySysId(sysId, warehouseSysId);
        }

        /// <summary>
        /// 作废工单
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("CancelWork")]
        public void CancelWork(CancelWorkDto request)
        {
            _workMangerAppService.CancelWork(request);
        }

        /// <summary>
        /// 更新工单
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("UpdateWorkInfo")]
        public void UpdateWorkInfo(WorkUpdateDto request)
        {
            _workMangerAppService.UpdateWorkInfo(request);
        }

        /// <summary>
        /// 创建工单
        /// </summary>
        /// <param name="dtoList"></param>
        [HttpPost, Route("AddWork")]
        public void AddWork(List<WorkDetailDto> dtoList)
        {
            _workMangerAppService.AddWork(dtoList);
        }


    }
}
