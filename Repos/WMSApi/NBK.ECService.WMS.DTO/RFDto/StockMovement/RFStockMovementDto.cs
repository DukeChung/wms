using System;

namespace NBK.ECService.WMS.DTO
{
    public class RFStockMovementDto : BaseDto
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid SkuSysId { get; set; }

        /// <summary>
        /// 商品UPC
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 来源货位
        /// </summary>
        public string FromLoc { get; set; }

        /// <summary>
        /// 来源批次
        /// </summary>
        public string FromLot { get; set; }

        /// <summary>
        /// 目标货位
        /// </summary>
        public string ToLoc { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 输入数量
        /// </summary>
        public decimal InputQty { get; set; }
    }
}
