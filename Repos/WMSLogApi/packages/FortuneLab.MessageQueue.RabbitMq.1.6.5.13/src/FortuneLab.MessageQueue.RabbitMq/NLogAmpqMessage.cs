using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FortuneLab.MessageQueue.RabbitMq
{
    [Serializable]
    public class NLogAmpqMessage
    {
        [JsonProperty("@source")]
        public Uri Source { get; set; }

        [JsonProperty("@timestamp")]
        public string TimeStampISO8601 { get; set; }

        [JsonProperty("@message")]
        public string Message { get; set; }

        [JsonProperty("@tags")]
        public HashSet<string> Tags { get; set; }

        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("@level")]
        public string Level { get; set; }

        [JsonProperty("@logger")]
        public string Logger { get; set; }

        [JsonProperty("@routingKey")]
        public string RoutingKey { get; set; }
    }
}
