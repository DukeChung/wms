using System;
using System.Runtime.Serialization;

namespace NBK.ECService.WMS.DTO.MQ.Log
{
    [Serializable]
    [DataContract]
    public class AccessLogDto
    {
        [DataMember]
        public Guid SysId { get; set; }
        [DataMember]
        public string app_controller { get; set; }
        [DataMember]
        public string app_service { get; set; }
        [DataMember]
        public string user_id { get; set; } = string.Empty;
        [DataMember]
        public string user_name { get; set; } = string.Empty;
        [DataMember]
        public string descr { get; set; }
        [DataMember]
        public DateTime create_date { get; set; }
        [DataMember]
        public DateTime start_time { get; set; }
        [DataMember]
        public DateTime end_time { get; set; }
        [DataMember]
        public decimal elapsed_time { get { return Convert.ToDecimal((end_time.Ticks - start_time.Ticks) / 10000000.00d); } }
        [DataMember]
        public string ip { get; set; }
        [DataMember]
        public string request_json { get; set; }
        [DataMember]
        public string response_json { get; set; }
        [DataMember]
        public bool flag { get; set; }
    }
}
