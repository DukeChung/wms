using NBK.ECService.WMSLog.Application.Interface;
using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSLog.Utility;
using Newtonsoft.Json;

namespace NBK.ECService.WMSLog.Application
{
    public  class PushMessageAppService: IPushMessageAppService
    {
        public PushMessageAppService()
        {
        }

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool PushMessage(PushMessageQuery query)
        {
            try
            {

                PushMessageDto push = new PushMessageDto();
                push.appkey = PublicConst.UmengAppKey;
                push.timestamp = StringHelper.GetTimeStamp();
                push.type = "broadcast";
                push.description = query.MessageDesc;

                push.payload = new PayloadDto()
                {
                    display_type = "notification"
                };
                push.payload.body = new BodyDto()
                {
                    ticker = query.MessageTitle,
                    title = query.MessageTitle,
                    text = query.MessageContent,
                    after_open = "go_app"
                };
                string str = JsonConvert.SerializeObject(push);

                string url = PublicConst.UmengPushUrl;
                string mysign = StringHelper.GetMD5Hash("POST" + url + str + PublicConst.UmengAppMasterSecret);
                url = url + "?sign=" + mysign;
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "POST";
                byte[] bs = Encoding.UTF8.GetBytes(str);
                request.ContentLength = bs.Length;
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                    reqStream.Close();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                HttpStatusCode statusCode = response.StatusCode;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    Stream myResponseStream = ((HttpWebResponse)e.Response).GetResponseStream();
                    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                    throw new Exception(myStreamReader.ReadToEnd());
                }
            }
            return true;
        }
    }

}
