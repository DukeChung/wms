using NBK.ECService.WMS.Utility;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace NBK.ECService.WMS.DTO.MQ.Log
{
    [Serializable]
    [DataContract]
    public class InterfaceLogDto
    {
        public InterfaceLogDto()
        {
            user_id = PublicConst.InterfaceUserId;
            user_name = PublicConst.InterfaceUserName;
            start_time = DateTime.Now;
        }

        public InterfaceLogDto(object request)
        {
            request_json = JsonConvert.SerializeObject(request);
            user_id = PublicConst.InterfaceUserId;
            user_name = PublicConst.InterfaceUserName;
            start_time = DateTime.Now;
        }

        public InterfaceLogDto(object request, string userId, string userName)
        {
            request_json = JsonConvert.SerializeObject(request);
            user_id = userId;
            user_name = userName;
            start_time = DateTime.Now;
        }

        [DataMember]
        public Guid? doc_sysId { get; set; }
        [DataMember]
        public string doc_order { get; set; }
        [DataMember]
        public string interface_type { get; set; }
        [DataMember]
        public string interface_name { get; set; }
        [DataMember]
        public string user_id { get; set; } = string.Empty;
        [DataMember]
        public string user_name { get; set; } = string.Empty;
        [DataMember]
        public string descr { get; set; }
        [DataMember]
        public string request_json { get; set; }
        [DataMember]
        public string response_json { get; set; }
        [DataMember]
        public DateTime create_date { get; set; }
        [DataMember]
        public DateTime start_time { get; set; }
        [DataMember]
        public DateTime end_time { get; set; }
        [DataMember]
        public decimal elapsed_time { get { return Convert.ToDecimal((end_time.Ticks - start_time.Ticks) / 10000000.00d); } }
        [DataMember]
        public bool flag { get; set; }
    }
}
