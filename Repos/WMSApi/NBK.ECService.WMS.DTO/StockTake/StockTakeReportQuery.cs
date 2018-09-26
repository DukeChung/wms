using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockTakeReportQuery : BaseQuery
    {
        public string SysIdStr { get; set; }

        public List<Guid> SysIds { get { return string.IsNullOrEmpty(SysIdStr) ? new List<Guid>() : SysIdStr.ToGuidList(); } }

        public string SkuUPCSearch { get; set; }

        public string SkuCodeSearch { get; set; }

        public string SkuNameSearch { get; set; }

        public bool? HasDiffSearch { get; set; }
    }
}
