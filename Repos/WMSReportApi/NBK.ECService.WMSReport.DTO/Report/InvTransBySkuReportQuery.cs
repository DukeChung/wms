using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.DTO.Base;

namespace NBK.ECService.WMSReport.DTO
{
    public class InvTransBySkuReportQuery : BaseQuery
    {
        public string SkuNameSearch { get; set; }

        public string SkuUPCSearch { get; set; }

        public string SkuCodeSearch { get; set; }

        public Guid? SkuSysIdSearch { get; set; }
    }
}
