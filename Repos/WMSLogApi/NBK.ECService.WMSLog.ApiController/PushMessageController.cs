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
    [RoutePrefix("api/PushMessage")]
    public class PushMessageController: AbpApiController
    {
        private IPushMessageAppService _pushMessageAppService;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushMessageAppService"></param>
        public PushMessageController(IPushMessageAppService pushMessageAppService)
        {
            _pushMessageAppService = pushMessageAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void PushMessageAPI() { }


        /// <summary>
        /// 友盟消息推送
        /// </summary>
        /// <param name="query"></param>
        [HttpPost, Route("PushMessage")]
        public void PushMessage(PushMessageQuery query)
        {
            _pushMessageAppService.PushMessage(query);
        }

    }
}
