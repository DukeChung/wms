using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Model.Models
{
    public class message : SysIdEntity
    {
        public int system_id { get; set; }

        public string message_type { get; set; }

        public DateTime create_date { get; set; }

        public int create_user_id { get; set; }

        public string create_user_name { get; set; }

        public string content { get; set; }

        public string groups { get; set; }

        public DateTime? start_time { get; set; }

        public DateTime? end_time { get; set; }

        public int receive_user_id { get; set; }

        public string receive_user_name { get; set; }

        public DateTime? receive_date { get; set; }

        public int status { get; set; }
    }
}
