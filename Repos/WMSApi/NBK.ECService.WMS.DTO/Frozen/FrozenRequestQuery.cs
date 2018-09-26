using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class FrozenRequestQuery : BaseQuery
    {
        public int Type { get; set; }

        public Guid? Zone { get; set; }

        public string Loc { get; set; }

        public string UPC { get; set; }

        public string SkuName { get; set; }

        public Guid StockFrozenSysId { get; set; }
    }
}
