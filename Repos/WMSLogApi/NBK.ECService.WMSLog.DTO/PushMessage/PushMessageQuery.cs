using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class PushMessageQuery
    {
        /// <summary>
        /// 推送类型
        /// </summary>
        public int PushType { get; set; }
        /// <summary>
        /// 消息标题
        /// </summary>
        public string  MessageTitle { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string MessageDesc { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string MessageContent { get; set; }
    }
}
