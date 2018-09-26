using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class MessageDto
    {
        public Guid SysId { get; set; }

        public string message_type { get; set; }

        public string content { get; set; }

        public string groups { get; set; }

        public DateTime create_date { get; set; }

        public int create_user_id { get; set; }

        public string create_user_name { get; set; }

        public DateTime? receive_date { get; set; }

        public string source { get; set; }

        public int receive_user_id { get; set; }

        public string receive_user_name { get; set; }

        public int status { get; set; }
    }
    
}
