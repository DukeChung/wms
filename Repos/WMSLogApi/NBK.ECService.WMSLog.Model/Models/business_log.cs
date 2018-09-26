using Abp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace NBK.ECService.WMSLog.Model.Models
{
    public partial class business_log : SysIdEntity
    {
        public Nullable<System.Guid> access_log_sysId { get; set; }
        public Guid? doc_sysId { get; set; }
        public string doc_order { get; set; }
        public string business_type { get; set; }
        public string business_name { get; set; }
        public string business_operation { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string descr { get; set; }
        public string request_json { get; set; }
        public string old_json { get; set; }
        public string new_json { get; set; }
        public System.DateTime create_date { get; set; }
        public bool flag { get; set; }
        public int system_id { get; set; }
        public virtual access_log access_log { get; set; }
    }
}
