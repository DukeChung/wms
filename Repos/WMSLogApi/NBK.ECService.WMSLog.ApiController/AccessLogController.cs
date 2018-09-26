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
    [RoutePrefix("api/AccessLog")]
    public class AccessLogController : AbpApiController
    {
        private IAccessLogAppService _accessLogAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessLogAppService"></param>
        public AccessLogController(IAccessLogAppService accessLogAppService)
        {
            _accessLogAppService = accessLogAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void AccessLogApi() { }

        /// <summary>
        /// 记录访问日志
        /// </summary>
        /// <param name="accessLogDto"></param>
        [HttpPost, Route("WriteAccessLog")]
        public void WriteAccessLog(AccessLogDto accessLogDto)
        {
            _accessLogAppService.WriteAccessLog(accessLogDto);
        }

        /// <summary>
        /// 获取首页业务API访问统计
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="latestDays"></param>
        /// <returns></returns>
        [HttpGet, Route("GetHomePageAccessLogStatistic")]
        public LogStatisticBaseDto GetHomePageAccessLogStatistic(int systemId, int latestDays)
        {
            return _accessLogAppService.GetHomePageAccessLogStatistic(systemId, latestDays);
        }

        /// <summary>
        /// 获取访问日志详情
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetAccessLogViewDto")]
        public AccessLogDto GetAccessLogViewDto(Guid sysId)
        {
            return _accessLogAppService.GetAccessLogViewDto(sysId);
        }

        /// <summary>
        /// 获取访问日志列表
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="accessLogQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetAccessLogList")]
        public Pages<AccessLogListDto> GetAccessLogList(int systemId, AccessLogQuery accessLogQuery)
        {
            return _accessLogAppService.GetAccessLogList(systemId, accessLogQuery);
        }
    }
}
