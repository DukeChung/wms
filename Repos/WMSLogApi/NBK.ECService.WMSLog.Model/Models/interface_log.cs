using Abp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace NBK.ECService.WMSLog.Model.Models
{
    public partial class interface_log : SysIdEntity
    {
        public Guid? doc_sysId { get; set; }
        public string doc_order { get; set; }
        public string interface_type { get; set; }
        public string interface_name { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string descr { get; set; }
        public string request_json { get; set; }
        public string response_json { get; set; }
        public System.DateTime create_date { get; set; }
        public System.DateTime start_time { get; set; }
        public System.DateTime end_time { get; set; }
        public decimal elapsed_time { get; set; }
        public bool flag { get; set; }
        public int system_id { get; set; }
    }
}
