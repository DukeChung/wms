using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockTakeSkuListDto
    {
        public Guid SysId { get; set; }

        public string SkuCode { get; set; }

        public string SkuUPC { get; set; }

        public string SkuName { get; set; }

        public string Loc { get; set; }
    }
}
