using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FortuneLab.MessageQueue.RabbitMq
{
    [Serializable]
    public class NLogAmpqMessage<T> : NLogAmpqMessage
        where T : class
    {
        [JsonProperty("@fields")]
        public T Fields { get; set; }
    }
}
