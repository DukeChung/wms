using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FortuneLab.MessageQueue.RabbitMq
{
    [Serializable]
    public class RequestMessageBody
    {
        [JsonProperty(PropertyName = "Application")]
        public string Application { get; set; }

        [JsonProperty(PropertyName = "Logged")]
        public DateTime Logged { get; set; }

        [JsonProperty(PropertyName = "level")]
        public string Level { get; set; }

        [JsonProperty(PropertyName = "Username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "ServerName")]
        public string ServerName { get; set; }

        [JsonProperty(PropertyName = "Https")]
        public int? Https { get; set; }

        [JsonProperty(PropertyName = "Port")]
        public int? Port { get; set; }

        [JsonProperty(PropertyName = "PathInfo")]
        public string PathInfo { get; set; }

        [JsonProperty(PropertyName = "QueryString")]
        public string QueryString { get; set; }

        [JsonProperty(PropertyName = "ServerAddress")]
        public string ServerAddress { get; set; }

        [JsonProperty(PropertyName = "RemoteAddress")]
        public string RemoteAddress { get; set; }

        [JsonProperty(PropertyName = "Logger")]
        public string Logger { get; set; }

        [JsonProperty(PropertyName = "Callsite")]
        public string Callsite { get; set; }

        [JsonProperty(PropertyName = "Duration")]
        public int? Duration { get; set; }

        [JsonProperty(PropertyName = "TraceId")]
        public string TraceId { get; set; }

        [JsonProperty(PropertyName = "Message")]
        public string Message { get; set; }

        [JsonIgnore]
        public string Exception { get; set; }
    }
}
