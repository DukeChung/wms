using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class PushMessageDto
    {
        /// <summary>
        /// 应用唯一标识
        /// </summary>
        public string appkey { get; set; }
        /// <summary>
        /// 时间戳，10位或者13位均可，时间戳有效期为10分钟
        /// </summary>
        public string timestamp { get; set; }
        /// <summary>
        /// 消息发送类型,其值可以为: broadcast-广播
        /// </summary>
        public string type { get; set; }
        public PayloadDto payload { get; set; }

        public PolicyDto policy{ get; set; }
        public string description { get; set; }
    }

    public class PayloadDto {
        /// <summary>
        /// 消息类型，值可以为:notification-通知，message-消息
        /// </summary>
        public string display_type { get; set; }

        public BodyDto body { get; set; }
    }

    public class PolicyDto {
        /// <summary>
        /// 过期时间，不设置
        /// </summary>
        public string expire_time{get;set;}
    }

    public class BodyDto
    {
        /// <summary>
        /// // 必填 通知栏提示文字
        /// </summary>
        public string ticker { get; set; }
        /// <summary>
        ///   // 必填 通知标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// // 必填 通知文字描述 
        /// </summary>
        public string text { get; set; }
        public string after_open { get; set; }
    }
}
