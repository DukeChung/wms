using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.Receipt
{
    public class ReceiptCollectionLotViewDto
    {
        public Guid ReceiptSysId { get; set; }

        public Guid SkuSysId { get; set; }

        public string SkuUPC { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public decimal? TotalReceivedQty { get; set; }

        public decimal? NoCollectionQty { get; set; }

        public List<ReceiptDetailViewDto> ReceiptDetailLotViewDtoList { get; set; }
    }
}
