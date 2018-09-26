using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class MonitorReportQuery: BaseQuery
    {  
        public string orderBy { get; set; }
        public string desc { get; set; }

    }
}
