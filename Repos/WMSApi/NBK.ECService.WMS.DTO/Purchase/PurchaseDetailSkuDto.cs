using System;

namespace NBK.ECService.WMS.DTO
{
    public class PurchaseDetailSkuDto:BaseDto
    {
        public Guid? PurchaseDetailSysId { get; set; }
        public Guid? PurchaseSysId { get; set; }
        public Guid? SkuSysId { get; set; }

        public string SkuCode { get; set; }
        public string SkuName { get; set; }
        public string SkuDescr { get; set; }
        public string SkuUPC { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string UPC { get; set; }
        public string RecommendLoc { get; set; }

        /// <summary>
        /// 保质期
        /// </summary>
        public int? DaysToExpire { get; set; }

        public Guid? UOMSysId { get; set; }

        public Guid? PackSysId { get; set; }

        public decimal? PurchasePrice { get; set; }

        public int? ReceivedQty { get; set; }

        public int? Qty { get; set; }
    }
}