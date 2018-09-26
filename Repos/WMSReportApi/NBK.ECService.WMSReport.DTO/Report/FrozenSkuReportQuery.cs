using NBK.ECService.WMSReport.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class FrozenSkuReportQuery : BaseQuery
    {
        public string UPC { get; set; }

        public string SkuName { get; set; }

        public bool IsStoreZero { get; set; }
    }
}
