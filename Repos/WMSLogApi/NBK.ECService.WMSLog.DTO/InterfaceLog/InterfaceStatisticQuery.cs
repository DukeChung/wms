using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO
{
    public class InterfaceStatisticQuery : BaseQuery
    {

        public DateTime? CreateDateFrom { get; set; }

        public DateTime? CreateDateTo { get; set; }
    }
}
