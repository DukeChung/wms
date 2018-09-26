using System;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
   
    public class OutStockOrderItemDetailDto
    { 
        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductCode { get; set; }
 
        /// <summary>
        /// 商品出库数量
        /// </summary>
        public int? Quantity { get; set; } 
    }
}