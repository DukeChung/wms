using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class OutboundPrePackDiffDto
    {
        public OutboundPrePackDiffDto()
        {
            DetailDiffList = new List<DiffDto>();
        }

        public string StorageLoc { get; set; }

        public List<DiffDto> DetailDiffList { get; set; }
    }

    public class DiffDto
    {
        public string UPC { get; set; }

        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public string UOMCode { get; set; }

        public decimal OutboundDisplayQty { get; set; }

        public decimal PrePackDisplayQty { get; set; }

        public bool MoreOrLess { get; set; }

        public string Memo { get; set; }
    }
}
