using System;
using System.Runtime.Serialization;

namespace NBK.ECService.WMS.DTO.MQ.Log
{
    [Serializable]
    [DataContract]
    public class BusinessLogDto
    {
        [DataMember]
        public Guid access_log_sysId { get; set; }
        [DataMember]
        public Guid? doc_sysId { get; set; }
        [DataMember]
        public string doc_order { get; set; }
        [DataMember]
        public string business_type { get; set; }
        [DataMember]
        public string business_name { get; set; }
        [DataMember]
        public string business_operation { get; set; }
        [DataMember]
        public string user_id { get; set; } = string.Empty;
        [DataMember]
        public string user_name { get; set; } = string.Empty;
        [DataMember]
        public string descr { get; set; }
        [DataMember]
        public string request_json { get; set; }
        [DataMember]
        public string old_json { get; set; }
        [DataMember]
        public string new_json { get; set; }
        [DataMember]
        public DateTime create_date { get; set; }
        [DataMember]
        public bool flag { get; set; }
    }
}
