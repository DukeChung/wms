namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartySkuPackDto
    {
        /// <summary>
        /// Delete Insert Update
        /// </summary>
        public string Action { get; set; }

        /// <summary>
       /// 商品ID
       /// </summary>
        public string SkuOtherId { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string UOMName { get; set; }
       /// <summary>
       /// 对应UPC
       /// </summary>
        public string UPC { get; set; }
        /// <summary>
        /// 包装数量
        /// </summary>
        public int? PackQty { get; set; }

        public int? CoefficientID { get; set; }
    }
}