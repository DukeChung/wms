using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.MessageQueue.MessageModels
{
    public class StatusChangeBusinessEventData<T> : BusinessEventData<T>
    {
        /// <summary>
        /// 处理后的新状态
        /// </summary>
        public string StatusType { get; set; }

        /// <summary>
        /// 本次状态变化之前的状态
        /// </summary>
        public string FromStatus { get; set; }
    }
}
