using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.MQ;
using NBK.ECService.WMS.DTO.Outbound;
using NBK.ECService.WMS.DTO.ThirdParty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMS.ApiController.MQControllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/MQ/OrderManagement/Work")]
    [AccessLog]
    public class MQWorkController : AbpApiController
    {
        private IWorkMangerAppService _workMangerAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workMangerAppService"></param>
        public MQWorkController(IWorkMangerAppService workMangerAppService)
        {
            _workMangerAppService = workMangerAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void MQWorkAPI() { }

        /// <summary>
        /// 修改工单状态(消息队列调用)
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("MQUpdateWorkByDocOrder")]
        public CommonResponse MQUpdateWorkByDocOrder(MQWorkDto request)
        {
            return _workMangerAppService.UpdateWorkByDocOrder(request);
        }

        /// <summary>
        /// 增加工单(消息队列调用)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("MQAddWork")]
        public CommonResponse MQAddWork(MQWorkDto request)
        {
            return _workMangerAppService.MQAddWork(request);
        }
    }
}
