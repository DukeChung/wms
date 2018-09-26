using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class SkuDto
    {
        public Guid SysId { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public Guid SkuClassSysId { get; set; }

        public string SkuClassName { get; set; }

        public long? ShelfLifeCodeType { get; set; }

        public string SkuDescr { get; set; }

        public int? ShelfLifeOnReceiving { get; set; }

        public int? ShelfLife { get; set; }

        public Guid PackSysId { get; set; }

        public int? DaysToExpire { get; set; }

        public Guid LotTemplateSysId { get; set; }

        public bool? ShelfLifeIndicator { get; set; }

        public decimal? Length { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        public decimal? Cube { get; set; }

        public decimal? NetWeight { get; set; }

        public decimal? GrossWeight { get; set; }

        public decimal? CostPrice { get; set; }

        public decimal? SalePrice { get; set; }

        public bool? Fresh { get; set; }

        public bool? FragileArticles { get; set; }

        public string Image { get; set; }

        public string Color { get; set; }

        public string Style { get; set; }

        public long CreateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public long UpdateBy { get; set; }

        public DateTime UpdateDate { get; set; }

        public bool IsActive { get; set; }

        public string OtherId { get; set; }

        public string UPC { get; set; }

        public string OtherUPC1 { get; set; }

        public string OtherUPC2 { get; set; }

        public string OtherUPC3 { get; set; }

        public string OtherUPC4 { get; set; }

        public string RecommendLoc { get; set; }

        /// <summary>
        /// 是否是原材料
        /// </summary>
        public bool? IsMaterial { get; set; }

        public int SpecialTypes { get; set; }
    }
}
