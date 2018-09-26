using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO.Other
{
    public class PurchaseDetailViewDto : PurchaseDetailDto
    {
        public string SkuCode { get; set; }
        public string SkuName { get; set; }
        public string SkuDescr { get; set; }
        public string SkuUPC { get; set; }
        public bool? IsMaterial { get; set; }

        /// <summary>
        /// 本次接收数量
        /// </summary>
        public int CurrentQty { get; set; }

        public decimal DisplayCurrentQty { get; set; }

        /// <summary>
        /// 扫描数量
        /// </summary>
        public int ScanQty { get; set; }

        /// <summary>
        /// 包装系数
        /// </summary>
        public string PackFactor { get; set; }

        public LotTemplateDto LotTemplateDto { get; set; }

        public string UPC01 { get; set; }

        public string UPC02 { get; set; }

        public string UPC03 { get; set; }

        public string UPC04 { get; set; }

        public string UPC05 { get; set; }
        public int GiftQty { get; set; }

        public int? FieldValue01 { get; set; }

        public int? FieldValue02 { get; set; }

        public int? FieldValue03 { get; set; }

        public int? FieldValue04 { get; set; }

        public int? FieldValue05 { get; set; }
    }
}
