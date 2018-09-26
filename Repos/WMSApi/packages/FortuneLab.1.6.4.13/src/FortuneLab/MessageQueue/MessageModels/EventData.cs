using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.MessageQueue.MessageModels
{
    public class EventData : IEventMessageData
    {
        public EventData()
        {
            this.SerializeType = EventDataSerializeEnum.Json;
            this.SupportRetry = true;
        }

        /// <summary>
        /// 消息唯一编号
        /// </summary>
        public Guid MessageId { get; set; }

        /// <summary>
        /// 事件数据模型版本
        /// </summary>
        public int EventDataVersion { get; set; }

        /// <summary>
        /// 事件数据发送到MQ时的序列化形式
        /// </summary>
        public EventDataSerializeEnum SerializeType { get; set; }

        public virtual bool SupportRetry { get; set; }
    }

    /// <summary>
    /// 事件数据序列化类型
    /// </summary>
    public enum EventDataSerializeEnum
    {
        /// <summary>
        /// Json格式
        /// </summary>
        Json,
        /// <summary>
        /// 二进制格式
        /// </summary>
        Binary
    }
}
