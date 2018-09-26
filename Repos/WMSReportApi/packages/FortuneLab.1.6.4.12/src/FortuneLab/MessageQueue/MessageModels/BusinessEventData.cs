using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.MessageQueue.MessageModels
{
    public class BusinessEventData<T> : EventData
    {
        public BusinessEventData()
        {

        }

        /// <summary>
        /// 事件数据上下文
        /// 这里要考虑对应数据的序列化与反序列化, 暂时支持Json与二进制
        /// </summary>
        public T EventDataContext { get; set; }
    }
}
