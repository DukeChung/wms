using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class StockTransferDto : BaseDto
    {
        public Guid? SysId { get; set; }

        public string StockTransferOrder { get; set; }
        public string Descr { get; set; }

        public Guid FromSkuSysId { get; set; }

        public Guid ToSkuSysId { get; set; }

        public string SkuName { get; set; }

        public string SkuCode { get; set; }

        public string SkuDescr { get; set; }

        public string UPC { get; set; }

        public string WarehouseName { get; set; }

        public string FromLoc { get; set; }

        public string ToLoc { get; set; }

        public string FromExternalLot { get; set; }

        public string ToExternalLot { get; set; }

        public string FromLot { get; set; }

        public string ToLot { get; set; }

        public int Status { get; set; }

        public string StatusDisplay
        {
            get
            {
                if (Status == 0)
                    return string.Empty;
                return ((StockTransferStatus)Status).ToDescription();
            }
        }

        public int CurrentQty { get; set; }

        public decimal DisplayCurrentQty { get; set; }

        public int FromQty { get; set; }

        public decimal DisplayFromQty { get; set; }

        public int ToQty { get; set; }

        public decimal DisplayToQty { get; set; }

        public DateTime? FromProduceDate { get; set; }

        public string FromProduceDateDisplay
        {
            get
            {
                return FromProduceDate.HasValue ? FromProduceDate.Value.ToString(PublicConst.DateFormat) : string.Empty;
            }
        }

        public DateTime? ToProduceDate { get; set; }

        public string ToProduceDateDisplay
        {
            get
            {
                return ToProduceDate.HasValue ? ToProduceDate.Value.ToString(PublicConst.DateFormat) : string.Empty;
            }
        }

        public DateTime? FromExpiryDate { get; set; }

        public string FromExpiryDateDisplay
        {
            get
            {
                return FromExpiryDate.HasValue ? FromExpiryDate.Value.ToString(PublicConst.DateFormat) : string.Empty;
            }
        }

        public DateTime? ToExpiryDate { get; set; }

        public string ToExpiryDateDisplay
        {
            get
            {
                return ToExpiryDate.HasValue ? ToExpiryDate.Value.ToString(PublicConst.DateFormat) : string.Empty;
            }
        }

        public string FromLotAttr01 { get; set; }

        public string FromLotAttr02 { get; set; }

        public string FromLotAttr03 { get; set; }

        public string FromLotAttr04 { get; set; }

        public string FromLotAttr05 { get; set; }

        public string FromLotAttr06 { get; set; }

        public string FromLotAttr07 { get; set; }

        public string FromLotAttr08 { get; set; }

        public string FromLotAttr09 { get; set; }


        public string ToLotAttr01 { get; set; }

        public string ToLotAttr02 { get; set; }

        public string ToLotAttr03 { get; set; }

        public string ToLotAttr04 { get; set; }

        public string ToLotAttr05 { get; set; }

        public string ToLotAttr06 { get; set; }

        public string ToLotAttr07 { get; set; }

        public string ToLotAttr08 { get; set; }

        public string ToLotAttr09 { get; set; }

        public string LotAttrName01 { get; set; }

        public string LotAttrName02 { get; set; }

        public string LotAttrName03 { get; set; }

        public string LotAttrName04 { get; set; }

        public string LotAttrName05 { get; set; }

        public string LotAttrName06 { get; set; }

        public string LotAttrName07 { get; set; }

        public string LotAttrName08 { get; set; }

        public string LotAttrName09 { get; set; }

        public bool? LotVisible01 { get; set; }

        public bool? LotVisible02 { get; set; }

        public bool? LotVisible03 { get; set; }

        public bool? LotVisible04 { get; set; }

        public bool? LotVisible05 { get; set; }

        public bool? LotVisible06 { get; set; }

        public bool? LotVisible07 { get; set; }

        public bool? LotVisible08 { get; set; }

        public bool? LotVisible09 { get; set; }


        public Nullable<bool> InLabelUnit01 { get; set; }
        public Nullable<int> FieldValue01 { get; set; }
        public Nullable<int> FieldValue02 { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
