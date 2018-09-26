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
    [RoutePrefix("api/SummaryLog")]
    public class SummaryLogController : AbpApiController
    {
        private ISummaryLogAppService _summaryLogAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="summaryLogAppService"></param>
        public SummaryLogController(ISummaryLogAppService summaryLogAppService)
        {
            _summaryLogAppService = summaryLogAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void SummaryLogApi() { }

        /// <summary>
        /// 获取首页日志列表
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="summaryLogQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetHomePageSummaryLog")]
        public List<SummaryLogDto> GetHomePageSummaryLog(int systemId, SummaryLogQuery summaryLogQuery)
        {
            return _summaryLogAppService.GetHomePageSummaryLog(systemId, summaryLogQuery);
        }

        /// <summary>
        /// 获取首页最耗时接口列表
        /// </summary>
        /// <param name="systemId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetHomePageMaxElapsedTimeLog")]
        public List<SummaryLogDto> GetHomePageMaxElapsedTimeLog(int systemId)
        {
            return _summaryLogAppService.GetHomePageMaxElapsedTimeLog(systemId);
        }

        /// <summary>
        /// 获取首页X时间内访问最多接口列表
        /// </summary>
        /// <param name="systemId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetHomePageMaxFrequencyLog")]
        public MaxFrequencyDto GetHomePageMaxFrequencyLog(int systemId)
        {
            return _summaryLogAppService.GetHomePageMaxFrequencyLog(systemId);
        }

        /// <summary>
        /// 获取首页X时间内访问最多接口明细
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="descr"></param>
        /// <returns></returns>
        [HttpGet, Route("GetFrequencyDetailDto")]
        public MaxFrequencyLogDto GetFrequencyDetailDto(int systemId, string descr)
        {
            return _summaryLogAppService.GetFrequencyDetailDto(systemId, descr);
        }

        /// <summary>
        /// 获取日志列表
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="dtoQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetInterfaceStatisticByPage")]
        public Pages<InterfaceStatisticList> GetInterfaceStatisticByPage(int systemId, InterfaceStatisticQuery dtoQuery)
        {
            return _summaryLogAppService.GetInterfaceStatisticByPage(systemId, dtoQuery);
        }
    }
}
