using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockMovementSkuQuery : BaseQuery
    {
        public string SkuUPCSearch { get; set; }

        public string SkuNameSearch { get; set; }

        public string LocSearch { get; set; }

        public string LotSearch { get; set; }
    }
}
