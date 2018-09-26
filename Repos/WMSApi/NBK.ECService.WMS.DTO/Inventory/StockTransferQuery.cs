using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockTransferQuery : BaseQuery
    {
        public string SkuName { get; set; }

        public string UPC { get; set; }

        public string Lot { get; set; }

        public string ExternalLot { get; set; }

        public int? Status { get; set; }

        public DateTime? ProduceDateFrom { get; set; }

        public DateTime? ProduceDateTo { get; set; }

        public DateTime? ExpiryDateFrom { get; set; }

        public DateTime? ExpiryDateTo { get; set; }

        public string LotAttr01 { get; set; }

        public string LotAttr02 { get; set; }

        public string LotAttr03 { get; set; }

        public string LotAttr04 { get; set; }

        public string LotAttr05 { get; set; }

        public string LotAttr06 { get; set; }

        public string LotAttr07 { get; set; }

        public string LotAttr08 { get; set; }

        public string LotAttr09 { get; set; }

        public bool GreaterThanZero { get; set; }
    }
}
