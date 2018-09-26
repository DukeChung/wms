using FortuneLab.WebClient.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebClient.Models
{
    public class SessionObject //: ResultWithErrorInfo
    {
        public string SessionKey { get; set; }
        public  User LogonUser { get; set; }
    }
}
