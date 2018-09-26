using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockTransferLotListDto
    {
        public Guid SysId { get; set; }

        public Guid SkuSysId { get; set; }

        public string SkuName { get; set; }

        public string SkuCode { get; set; }

        public string SkuDescr { get; set; }

        public string UPC { get; set; }

        public string WarehouseName { get; set; }

        public string Loc { get; set; }

        public string Lot { get; set; }

        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }

        public int AvailableQty { get; set; }

        public decimal DisplayAvailableQty { get; set; }

        public DateTime? ProduceDate { get; set; }

        public string ProduceDateDisplay
        {
            get
            {
                return ProduceDate.HasValue ? ProduceDate.Value.ToString(PublicConst.DateFormat) : string.Empty;
            }
        }

        public DateTime? ExpiryDate { get; set; }

        public string ExpiryDateDisplay
        {
            get
            {
                return ExpiryDate.HasValue ? ExpiryDate.Value.ToString(PublicConst.DateFormat) : string.Empty;
            }
        }

        public string LotAttr01 { get; set; }

        public string LotAttr02 { get; set; }

        public string LotAttr03 { get; set; }

        public string LotAttr04 { get; set; }

        public string LotAttr05 { get; set; }

        public string LotAttr06 { get; set; }

        public string LotAttr07 { get; set; }

        public string LotAttr08 { get; set; }

        public string LotAttr09 { get; set; }

    }
}
