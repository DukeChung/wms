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
    [RoutePrefix("api/AsynBussinessProcessLog")]
    public class AsynBussinessProcessLogController : AbpApiController
    {
        private IAsynBussinessProcessLogAppService _appService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessLogAppService"></param>
        public AsynBussinessProcessLogController(IAsynBussinessProcessLogAppService appService)
        {
            _appService = appService;
        }

        [HttpGet]
        public void AsynBussinessProcessLogApi() { }

        [HttpPost, Route("WriteLog")]
        public void WriteLog(AsynBussinessProcessLogDto request)
        {
            _appService.WriteLog(request);
        }
    }
}
