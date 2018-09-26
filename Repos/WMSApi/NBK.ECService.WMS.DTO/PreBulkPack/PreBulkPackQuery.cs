using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class PreBulkPackQuery : BaseQuery
    {
        public string SkuName { get; set; }

        public string UPC { get; set; }

        public int? Status { get; set; }

        public string StorageCase { get; set; }
        public string OutboundOrder { get; set; }
    }
}
