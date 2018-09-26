using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class AccessLogQuery : BaseQuery
    {
        public string DescrSearch { get; set; }

        public bool? FlagSearch { get; set; }

        public DateTime? CreateDateFromSearch { get; set; }

        public DateTime? CreateDateToSearch { get; set; }
    }
}
