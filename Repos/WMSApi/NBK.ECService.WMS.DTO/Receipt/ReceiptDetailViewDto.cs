using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.DTO
{
    public class ReceiptDetailViewDto : ReceiptDetailDto
    {
        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string SkuUPC { get; set; }

        public string OtherId { get; set; }

        public string SkuDescr { get; set; }

        public string UOMDescr { get; set; }

        //public int ShelvesStatus { get; set; }

        public string ShelvesStatusText { get; set; }

        public int ShelvesQty { get; set; }

        public decimal DisplayShelvesQty { get; set; }

        /// <summary>
        /// 领料分拣数量
        /// </summary>
        public decimal DisplayPickingQty { get; set; }

        /// <summary>
        /// 包装系数
        /// </summary>
        public string PackFactor { get; set; }

        public string UPC01 { get; set; }

        public string UPC02 { get; set; }

        public string UPC03 { get; set; }

        public string UPC04 { get; set; }

        public string UPC05 { get; set; }

        public int GiftQty { get; set; }
        /// <summary>
        /// 破损数量
        /// </summary>
        public int AdjustmentQty { get; set; }

        public int? FieldValue01 { get; set; }

        public int? FieldValue02 { get; set; }

        public int? FieldValue03 { get; set; }

        public int? FieldValue04 { get; set; }

        public int? FieldValue05 { get; set; }

        public bool IsMustLot { get; set; }

        public bool IsDefaultLot { get; set; }

        public string LotAttr01 { get; set; }

        public string LotAttr02 { get; set; }

        public string LotAttr04 { get; set; }

        public string LotAttr03 { get; set; }

        public string LotAttr05 { get; set; }

        public string LotAttr06 { get; set; }

        public string LotAttr07 { get; set; }

        public string LotAttr08 { get; set; }

        public string LotAttr09 { get; set; }

        public string ExternalLot { get; set; }

        public DateTime? ProduceDate { get; set; }

        public string ProduceDateText { get { return ProduceDate.HasValue ? ProduceDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public DateTime? ExpiryDate { get; set; }

        public string ExpiryDateText { get { return ExpiryDate.HasValue ? ExpiryDate.Value.ToString(PublicConst.DateFormat) : string.Empty; } }

        public LotTemplateDto LotTemplateDto { get; set; }
    }
}
