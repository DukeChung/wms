using System;

namespace NBK.ECService.WMS.DTO.ThirdParty
{

    public class InStockOrderItemDetailDto
    {

        /// <summary>
        /// 子单商品明细商品编号
        /// </summary>
        public int ProductCode { get; set; }

        /// <summary>
        /// 子单商品明细单品数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 子单商品已经入库的数量
        /// </summary>
        public int InStockQuantity { get; set; }

        /// <summary>
        /// 子单商品已经拒绝入库的数量
        /// </summary>
        public int RejectedQuantity { get; set; }

        /// <summary>
        /// 子单商品已经入库的赠品数量
        /// </summary>
        public int InStockGiftQuantity { get; set; }

        /// <summary>
        /// 子单商品已经拒绝入库的赠品数量
        /// </summary>
        public int RejectedGiftQuantity { get; set; }

        /// <summary>
        /// 子单商品已经入库的破损数量
        /// </summary>
        public int DamagedQuantity { get; set; }
    }
}