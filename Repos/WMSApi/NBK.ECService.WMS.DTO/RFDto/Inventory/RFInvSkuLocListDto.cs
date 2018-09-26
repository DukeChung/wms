using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class RFInvSkuLocListDto : BaseDto
    {
        public string SkuName { get; set; }

        public string Loc { get; set; }

        public int Qty { get; set; }

        public int AvailableQty { get; set; }

        public decimal DisplayQty { get; set; }

        public decimal DisplayAvailableQty { get; set; }
    }
}
