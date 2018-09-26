using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockTakeQuery : BaseQuery
    {
        public string StockTakeOrderSearch { get; set; }

        public int? StatusSearch { get; set; }

        public int? AssignBySearch { get; set; }

        public string SkuUPCSearch { get; set; }

        public string SkuCodeSearch { get; set; }

        public string SkuNameSearch { get; set; }

        public int? CreateBySearch { get; set; }

        public DateTime? StartTimeSearch { get; set; }

        public DateTime? EndTimeSearch { get; set; }
    }
}
