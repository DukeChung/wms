using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockMovementQuery : BaseQuery
    {
        public string SkuNameSearch { get; set; }

        public string SkuUPCSearch { get; set; }

        public string ToLocSearch { get; set; }

        public int? StatusSearch { get; set; }
    }
}
