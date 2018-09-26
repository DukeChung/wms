using Abp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace NBK.ECService.WMSLog.Model.Models
{
    public partial class access_log : SysIdEntity
    {
        public access_log()
        {
            this.business_log = new List<business_log>();
        }

        public string app_controller { get; set; }
        public string app_service { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string descr { get; set; }
        public System.DateTime create_date { get; set; }
        public System.DateTime start_time { get; set; }
        public System.DateTime end_time { get; set; }
        public decimal elapsed_time { get; set; }
        public string ip { get; set; }
        public string request_json { get; set; }
        public string response_json { get; set; }
        public bool flag { get; set; }
        public int system_id { get; set; }
        public virtual ICollection<business_log> business_log { get; set; }
    }
}
