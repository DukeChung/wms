using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO.Redis
{
    public class LoginUserInfo
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }

        public int RetryLoginCount { get; set; }

        public DateTime LoginDate { get; set; }
    }
}
