using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockMovementSkuDto
    {
        public Guid SysId { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public string UPC { get; set; }

        public string Loc { get; set; }

        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }

        public string Lot { get; set; }
    }
}
