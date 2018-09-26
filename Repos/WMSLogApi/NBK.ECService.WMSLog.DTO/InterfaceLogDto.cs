using NBK.ECService.WMSLog.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class InterfaceLogDto
    {
        public Guid? SysId { get; set; }
        public Guid? doc_sysId { get; set; }
        public string doc_order { get; set; }
        public string interface_type { get; set; }
        public string interface_name { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string descr { get; set; }
        public string request_json { get; set; }
        public string response_json { get; set; }
        public DateTime create_date { get; set; }
        public string create_date_text { get { return create_date.ToString(PublicConst.DateTimeFormat); } }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public decimal elapsed_time { get { return Convert.ToDecimal((end_time.Ticks - start_time.Ticks) / 10000000.00d); } }
        public bool flag { get; set; }
        public int system_id { get; set; }
    }
}
