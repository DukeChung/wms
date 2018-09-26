using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBK.WMSLog.Portal.Services
{
    public class PushMessageApiClient
    {
        private static readonly PushMessageApiClient instance = new PushMessageApiClient();

        private PushMessageApiClient() { }

        public static PushMessageApiClient GetInstance() { return instance; }

        /// <summary>
        /// 友盟消息推送
        /// </summary>
        /// <param name="query"></param>
        /// <param name="businessLogQuery"></param>
        /// <returns></returns>
        public ApiResponse<string> PushMessage(PushMessageQuery pushMessageQuery)
        {
            return ApiClient.Post<string>(PublicConst.WmsLogApiUrl, "/PushMessage/PushMessage", new CoreQuery(), pushMessageQuery);
        }

    }
}