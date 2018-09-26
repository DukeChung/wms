using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;

namespace NBK.ECService.WMSReport.DTO.Other
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

        //public string ShelvesStatusText { get { return ((ShelvesStatus)ShelvesStatus).ToDescription(); } }

        public string ShelvesStatusText
        {
            get
            {
                //未上架
                if (ShelvesQty == 0)
                {
                    return ShelvesStatus.NotOnShelves.ToDescription();
                }

                //上架中
                if (ReceivedQty.HasValue && ReceivedQty > ShelvesQty)
                {
                    return ShelvesStatus.Shelves.ToDescription();
                }

                //上架完成   
                if (ReceivedQty.HasValue && ReceivedQty == ShelvesQty)
                {
                    return ShelvesStatus.Finish.ToDescription();
                }
                return string.Empty;
            }
        }

        public int ShelvesQty { get; set; }

        public decimal DisplayShelvesQty { get; set; }

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

        public string LotAttr01 { get; set; }

        public LotTemplateDto LotTemplateDto { get; set; }
    }
}
