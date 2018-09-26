using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class AdjustmentQuery : BaseQuery
    {
        public string AdjustmentOrder { get; set; }

        public int? Status { get; set; }

        public int? Type { get; set; }

        public string CreateUserName { get; set; }

        public string UPC { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }
        public string SourceOrder { get; set; }
    }
}
