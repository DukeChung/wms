using Abp.WebApi.Controllers;
using NBK.ECService.WMSLog.Application.Interface;
using NBK.ECService.WMSLog.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMSLog.ApiController
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/InterfaceLog")]
    public class InterfaceLogController : AbpApiController
    {
        private IInterfaceLogAppService _interfaceLogAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interfaceLogAppService"></param>
        public InterfaceLogController(IInterfaceLogAppService interfaceLogAppService)
        {
            _interfaceLogAppService = interfaceLogAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void InterfaceLogApi() { }

        /// <summary>
        /// 记录接口日志
        /// </summary>
        /// <param name="interfaceLogDto"></param>
        [HttpPost, Route("WriteInterfaceLog")]
        public void WriteInterfaceLog(InterfaceLogDto interfaceLogDto)
        {
            _interfaceLogAppService.WriteInterfaceLog(interfaceLogDto);
        }

        /// <summary>
        /// 获取首页接口访问统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetHomePageInterfaceLogStatistic")]
        public LogStatisticBaseDto GetHomePageInterfaceLogStatistic(int systemId, int latestDays)
        {
            return _interfaceLogAppService.GetHomePageInterfaceLogStatistic(systemId, latestDays);
        }

        /// <summary>
        /// 获取接口日志详情
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetInterfaceLogViewDto")]
        public InterfaceLogDto GetInterfaceLogViewDto(Guid sysId)
        {
            return _interfaceLogAppService.GetInterfaceLogViewDto(sysId);
        }

        /// <summary>
        /// 获取接口日志列表
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="interfaceLogQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetInterfaceLogList")]
        public Pages<InterfaceLogListDto> GetInterfaceLogList(int systemId, InterfaceLogQuery interfaceLogQuery)
        {
            return _interfaceLogAppService.GetInterfaceLogList(systemId, interfaceLogQuery);
        }

        /// <summary>
        /// 接口调用失败时候手动调用
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="interfaceLogQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("InvokeInterfaceLogApi")]
        public bool InvokeInterfaceLogApi(int systemId, InterfaceLogQuery interfaceLogQuery)
        {
            return _interfaceLogAppService.InvokeInterfaceLogApi(systemId, interfaceLogQuery);
        }

        /// <summary>
        /// 重新调用插入出库单
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="interfaceLogQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("InsertOutbound")]
        public bool InsertOutbound(int systemId, InterfaceLogQuery interfaceLogQuery)
        {
            return _interfaceLogAppService.InsertOutbound(systemId, interfaceLogQuery);
        }
    }
}
