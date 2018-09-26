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
    [RoutePrefix("api/BusinessLog")]
    public class BusinessLogController : AbpApiController
    {
        private IBusinessLogAppService _businessLogAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="businessLogAppService"></param>
        public BusinessLogController(IBusinessLogAppService businessLogAppService)
        {
            _businessLogAppService = businessLogAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void BusinessLogApi() { }

        /// <summary>
        /// 记录业务日志
        /// </summary>
        /// <param name="businessLogDto"></param>
        [HttpPost, Route("WriteBusinessLog")]
        public void WriteBusinessLog(BusinessLogDto businessLogDto)
        {
            _businessLogAppService.WriteBusinessLog(businessLogDto);
        }

        /// <summary>
        /// 入库业务执行统计
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        [HttpGet, Route("GetInboundBizLogByDays")]
        public InboundBizLogDto GetInboundBizLogByDays(int systemId, int days)
        {
            return _businessLogAppService.GetInboundBizLogByDays(systemId, days);
        }

        /// <summary>
        /// 出库业务执行统计
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        [HttpGet, Route("GetOutboundBizLogByDays")]
        public OutboundBizLogDto GetOutboundBizLogByDays(int systemId, int days)
        {
            return _businessLogAppService.GetOutboundBizLogByDays(systemId, days);
        }

        /// <summary>
        /// 统计单位每分钟内接口调用次数
        /// </summary>
        /// <param name="systemId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetApiProcessResult")]
        public ApiProcessResultTotalDto GetApiProcessResult(int systemId)
        {
            return _businessLogAppService.GetApiProcessResult(systemId);
        }

        /// <summary>
        /// 统计单位x秒内接口调用次数
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="secondInterval"></param>
        /// <returns></returns>
        [HttpGet, Route("GetApiProcessResultBySecondInterval")]
        public ApiProcessResultTotalDto GetApiProcessResult(int systemId, int secondInterval)
        {
            return _businessLogAppService.GetApiProcessResult(systemId, secondInterval);
        }

        /// <summary>
        /// 获取业务日志详情
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetBusinessLogViewDto")]
        public BusinessLogDto GetBusinessLogViewDto(Guid sysId)
        {
            return _businessLogAppService.GetBusinessLogViewDto(sysId);
        }

        /// <summary>
        /// 获取业务日志列表
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="businessLogQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetBusinessLogList")]
        public Pages<BusinessLogListDto> GetBusinessLogList(int systemId, BusinessLogQuery businessLogQuery)
        {
            return _businessLogAppService.GetBusinessLogList(systemId, businessLogQuery);
        }
    }
}
